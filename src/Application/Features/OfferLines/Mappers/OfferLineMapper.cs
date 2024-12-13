﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under one or more agreements.
//     The .NET Foundation licenses this file to you under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-12-13
//     Last Modified: 2024-12-13
//     Description: 
//       Defines mapping methods between `OfferLine` entities and related DTOs/commands 
//       within the CleanArchitecture.Blazor application. This mapper facilitates 
//       conversions to support different operations, such as creating, updating, 
//       and projecting offerline data.
// </auto-generated>
//------------------------------------------------------------------------------

using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class OfferLineMapper
{
    public static partial OfferLineDto ToDto(OfferLine source);
    public static partial OfferLine FromDto(OfferLineDto dto);
    public static partial OfferLine FromEditCommand(AddEditOfferLineCommand command);
    public static partial OfferLine FromCreateCommand(CreateOfferLineCommand command);
    public static partial UpdateOfferLineCommand ToUpdateCommand(OfferLineDto dto);
    public static partial AddEditOfferLineCommand CloneFromDto(OfferLineDto dto);
    public static partial void ApplyChangesFrom(UpdateOfferLineCommand source, OfferLine target);
    public static partial void ApplyChangesFrom(AddEditOfferLineCommand source, OfferLine target);
    public static partial IQueryable<OfferLineDto> ProjectTo(this IQueryable<OfferLine> q);
}

