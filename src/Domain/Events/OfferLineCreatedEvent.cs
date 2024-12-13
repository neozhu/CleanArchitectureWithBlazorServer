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
//       Represents a domain event that occurs when a new offerline is created.
//       Used to signal other parts of the system that a new offerline has been added.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanArchitecture.Blazor.Domain.Events;

    public class OfferLineCreatedEvent : DomainEvent
    {
        public OfferLineCreatedEvent(OfferLine item)
        {
            Item = item;
        }

        public OfferLine Item { get; }
    }

