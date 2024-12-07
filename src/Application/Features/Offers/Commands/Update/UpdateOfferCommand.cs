﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under one or more agreements.
//     The .NET Foundation licenses this file to you under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-12-06
//     Last Modified: 2024-12-06
//     Description: 
//       This file defines the UpdateOfferCommand and its handler for updating 
//       an existing Offer entity within the CleanArchitecture.Blazor application. 
//       It includes caching invalidation logic to maintain data consistency and 
//       raises a domain event upon successful update to notify other parts of the system.
//     
//     Documentation:
//       https://docs.cleanarchitectureblazor.com/features/offer
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// Use `UpdateOfferCommand` to update an existing offer entity in the system. 
// The handler ensures that if the entity is found, the changes are applied and 
// the necessary domain event (`OfferUpdatedEvent`) is raised. Caching is also 
// invalidated to keep the offer list consistent.

using CleanArchitecture.Blazor.Application.Features.Offers.Caching;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Commands.Update;

public class UpdateOfferCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
            [Description("Customer id")]
    public int CustomerId {get;set;} 
    [Description("Offer date")]
    public DateTime OfferDate {get;set;} 
    [Description("Total amount")]
    public decimal TotalAmount {get;set;} 
    [Description("Status")]
    public string? Status {get;set;} 
    //[Description("Offer lines")]
    //public List<OfferLineDtoDto>? OfferLines {get;set;} 

      public string CacheKey => OfferCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => OfferCacheKey.Tags;

}

public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public UpdateOfferCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
    {

       var item = await _context.Offers.FindAsync(request.Id, cancellationToken);
       if (item == null)
       {
           return await Result<int>.FailureAsync($"Offer with id: [{request.Id}] not found.");
       }
       OfferMapper.ApplyChangesFrom(request, item);
	    // raise a update domain event
	   //item.AddDomainEvent(new OfferUpdatedEvent(item));
       await _context.SaveChangesAsync(cancellationToken);
       return await Result<int>.SuccessAsync(item.Id);
    }
}

