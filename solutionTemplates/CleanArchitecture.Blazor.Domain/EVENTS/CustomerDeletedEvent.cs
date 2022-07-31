// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.$safeprojectname$.Events;

    public class CustomerDeletedEvent : DomainEvent
    {
        public CustomerDeletedEvent(Customer item)
        {
            Item = item;
        }

        public Customer Item { get; }
    }

