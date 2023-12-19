// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Queries;

using static Testing;

public class KeyValuesQueryTests : TestBase
{
    [Test]
    public void ShouldNotNullKeyValuesQueryByName()
    {
        var query = new KeyValuesQueryByName(Picklist.Brand);
        var result = SendAsync(query);
        FluentActions.Invoking(() =>
            SendAsync(query)).Should().NotBeNull();
    }
}