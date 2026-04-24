// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mediator;

namespace CleanArchitecture.Blazor.Domain.Events;
/// <summary>
/// Notification carrying finalized audit trails for asynchronous persistence.
/// </summary>
public sealed record AuditTrailsReadyEvent(IReadOnlyCollection<AuditTrail> AuditTrails) : INotification;
