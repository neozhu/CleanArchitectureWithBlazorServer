// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.KeyValues.Queries
{
    using static Testing;
    public class KeyValuesQueryTests : TestBase
    {
        [Test]
        public void ShouldNotNullKeyValuesQueryByName()
        {
            var query = new KeyValuesQueryByName("Status");
            var result = SendAsync(query);
            FluentActions.Invoking(() =>
                SendAsync(query)).Should().NotBeNull();
        }
    }
}
