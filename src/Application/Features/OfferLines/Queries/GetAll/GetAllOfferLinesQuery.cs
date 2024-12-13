﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-12-13
//     Last Modified: 2024-12-13
//     Description: 
//       Defines a query to retrieve all offerlines from the database. The result 
//       is cached to improve performance and reduce database load for repeated 
//       queries.
// </auto-generated>
//------------------------------------------------------------------------------

using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using DocumentFormat.OpenXml.InkML;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.GetAll;

public class GetAllOfferLinesQuery : ICacheableRequest<IEnumerable<OfferLineDto>>
{
   public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
}

public class GetAllOfferLinesQueryHandler :
     IRequestHandler<GetAllOfferLinesQuery, IEnumerable<OfferLineDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllOfferLinesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OfferLineDto>> Handle(GetAllOfferLinesQuery request, CancellationToken cancellationToken)
    {
        //var data1 = await _context.OfferLines.ProjectTo()
        //                                        .AsNoTracking()
        //.ToListAsync(cancellationToken);

        var data = await _context.Offers
                .SelectMany(o => o.OfferLines)
                .ProjectTo()
                .ToListAsync(cancellationToken);

        return data;
    }
}


