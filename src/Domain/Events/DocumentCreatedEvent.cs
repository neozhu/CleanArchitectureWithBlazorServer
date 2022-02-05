// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Domain.Events;

public class DocumentCreatedEvent : DomainEvent
{
    public DocumentCreatedEvent(Document item)
    {
        Item = item;
    }

    public Document Item { get; }
}
