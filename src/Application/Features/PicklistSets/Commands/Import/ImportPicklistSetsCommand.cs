// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.Import;

public class ImportPicklistSetsCommand : ICacheInvalidatorRequest<Result>
{
    public ImportPicklistSetsCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }
    public string FileName { get; set; }
    public byte[] Data { get; set; }
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

 

public class ImportPicklistSetsCommandHandler :
    IRequestHandler<ImportPicklistSetsCommand, Result>
{
    private readonly IValidator<AddEditPicklistSetCommand> _addValidator;
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ImportPicklistSetsCommandHandler> _localizer;

    public ImportPicklistSetsCommandHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ImportPicklistSetsCommandHandler> localizer,
        IValidator<AddEditPicklistSetCommand> addValidator
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
        _addValidator = addValidator;
    }

     
#nullable disable warnings
    public async Task<Result> Handle(ImportPicklistSetsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var result = await _excelService.ImportAsync(request.Data,
            new Dictionary<string, Func<DataRow, PicklistSet, object?>>
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
                    new AddEditPicklistSetCommand
                        { Name = item.Name, Value = item.Value, Description = item.Description, Text = item.Text },
                    cancellationToken);
                if (validationResult.IsValid)
                {
                    var exist = await db.PicklistSets.AnyAsync(x => x.Name == item.Name && x.Value == item.Value,
                        cancellationToken);
                    if (exist) continue;

                    item.AddDomainEvent(new CreatedEvent<PicklistSet>(item));
                    await db.PicklistSets.AddAsync(item, cancellationToken);
                }
                else
                {
                    errorsOccurred = true;
                    errors.AddRange(validationResult.Errors.Select(e =>
                        $"{(!string.IsNullOrWhiteSpace(item.Name.ToString()) ? $"{item.Name} - " : string.Empty)}{e.ErrorMessage}"));
                }
            }

            if (errorsOccurred) return await Result.FailureAsync(errors.ToArray());

            await db.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync();
        }
    }
}
