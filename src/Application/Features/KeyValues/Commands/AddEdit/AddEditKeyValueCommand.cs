// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")] public int Id { get; set; }

    [Description("Name")] public Picklist Name { get; set; }

    [Description("Value")] public string? Value { get; set; }

    [Description("Text")] public string? Text { get; set; }

    [Description("Description")] public string? Description { get; set; }

    public TrackingState TrackingState { get; set; } = TrackingState.Unchanged;
    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.GetOrCreateTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<KeyValueDto, AddEditKeyValueCommand>(MemberList.None);
            CreateMap<AddEditKeyValueCommand, KeyValue>(MemberList.None);
        }
    }
}

public class AddEditKeyValueCommandHandler : IRequestHandler<AddEditKeyValueCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddEditKeyValueCommandHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditKeyValueCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var keyValue = await _context.KeyValues.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = keyValue ?? throw new NotFoundException($"KeyValue Pair  {request.Id} Not Found.");
            keyValue = _mapper.Map(request, keyValue);
            keyValue.AddDomainEvent(new UpdatedEvent<KeyValue>(keyValue));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
        else
        {
            var keyValue = _mapper.Map<KeyValue>(request);
            keyValue.AddDomainEvent(new UpdatedEvent<KeyValue>(keyValue));
            _context.KeyValues.Add(keyValue);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
    }
}