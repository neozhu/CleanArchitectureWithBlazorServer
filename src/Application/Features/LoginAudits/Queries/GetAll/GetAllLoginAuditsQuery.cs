// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetAll;

public class GetAllLoginAuditsQuery : ICacheableRequest<IEnumerable<LoginAuditDto>>
{
    public string CacheKey => LoginAuditCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => new[] { "LoginAudits" };
}

public class GetAllLoginAuditsQueryHandler : IRequestHandler<GetAllLoginAuditsQuery, IEnumerable<LoginAuditDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLoginAuditsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoginAuditDto>> Handle(GetAllLoginAuditsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.LoginAudits
            .OrderByDescending(x => x.LoginTimeUtc)
            .Take(1000) // Limit to latest 1000 records for performance
            .ProjectTo<LoginAuditDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        return data;
    }
}
