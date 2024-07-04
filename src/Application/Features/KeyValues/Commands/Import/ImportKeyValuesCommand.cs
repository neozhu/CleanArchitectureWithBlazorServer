// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommand : ICacheInvalidatorRequest<Result>
{
    public ImportKeyValuesCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }

    public string FileName { get; set; }
    public byte[] Data { get; set; }
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.GetOrCreateTokenSource();
}

public record CreateKeyValueTemplateCommand : IRequest<byte[]>
{
}

public class ImportKeyValuesCommandHandler :
    IRequestHandler<CreateKeyValueTemplateCommand, byte[]>,
    IRequestHandler<ImportKeyValuesCommand, Result>
{
    private readonly IValidator<AddEditKeyValueCommand> _addValidator;
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ImportKeyValuesCommandHandler> _localizer;
    private readonly IMapper _mapper;

    public ImportKeyValuesCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ImportKeyValuesCommandHandler> localizer,
        IValidator<AddEditKeyValueCommand> addValidator
    )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
        _addValidator = addValidator;
    }

    public async Task<byte[]> Handle(CreateKeyValueTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[]
        {
            _localizer["Name"],
            _localizer["Value"],
            _localizer["Text"],
            _localizer["Description"]
        };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer["KeyValues"]);
        return result;
    }
#nullable disable warnings
    public async Task<Result> Handle(ImportKeyValuesCommand request, CancellationToken cancellationToken)
    {
        var result = await _excelService.ImportAsync(request.Data,
            new Dictionary<string, Func<DataRow, KeyValue, object?>>
            {
                {
                    _localizer["Name"],
                    (row, item) =>
                        item.Name = (Picklist)Enum.Parse(typeof(Picklist), row[_localizer["Name"]].ToString())
                },
                { _localizer["Value"], (row, item) => item.Value = row[_localizer["Value"]]?.ToString() },
                { _localizer["Text"], (row, item) => item.Text = row[_localizer["Text"]]?.ToString() },
                {
                    _localizer["Description"],
                    (row, item) => item.Description = row[_localizer["Description"]]?.ToString()
                }
            }, _localizer["Data"]);

        if (result is not { Succeeded: true, Data: not null }) return await Result.FailureAsync(result.Errors);
        {
            var importItems = result.Data;
            var errors = new List<string>();
            var errorsOccurred = false;
            foreach (var item in importItems)
            {
                var validationResult = await _addValidator.ValidateAsync(
                    new AddEditKeyValueCommand
                        { Name = item.Name, Value = item.Value, Description = item.Description, Text = item.Text },
                    cancellationToken);
                if (validationResult.IsValid)
                {
                    var exist = await _context.KeyValues.AnyAsync(x => x.Name == item.Name && x.Value == item.Value,
                        cancellationToken);
                    if (exist) continue;

                    item.AddDomainEvent(new CreatedEvent<KeyValue>(item));
                    await _context.KeyValues.AddAsync(item, cancellationToken);
                }
                else
                {
                    errorsOccurred = true;
                    errors.AddRange(validationResult.Errors.Select(e =>
                        $"{(!string.IsNullOrWhiteSpace(item.Name.ToString()) ? $"{item.Name} - " : string.Empty)}{e.ErrorMessage}"));
                }
            }

            if (errorsOccurred) return await Result.FailureAsync(errors.ToArray());

            await _context.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync();
        }
    }
}