// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Services;
using Xunit;

namespace CleanArchitecture.Blazor.Infrastructure.UnitTests.Services;

public class SecurityAnalysisHeuristicsTests
{
    [Theory]
    [InlineData("192.168.1.15", "192.168.1.*")]
    [InlineData("10.0.0.200", "10.0.0.*")]
    public void NormalizeIpForHeuristic_Ipv4_ReturnsSlash24(string ip, string expected)
    {
        var normalized = SecurityAnalysisHeuristics.NormalizeIpForHeuristic(ip);
        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void ExtractUserAgentCore_ChromeFullVersion_ReturnsMajor()
    {
        var ua = "Mozilla/5.0 Chrome/118.0.5993.70 Safari/537.36";
        var core = SecurityAnalysisHeuristics.ExtractUserAgentCore(ua);
        Assert.Equal("Chrome/118", core);
    }

    [Fact]
    public void ExtractUserAgentCore_Firefox_ReturnsMajor()
    {
        var ua = "Mozilla/5.0 Firefox/121.1";
        var core = SecurityAnalysisHeuristics.ExtractUserAgentCore(ua);
        Assert.Equal("Firefox/121", core);
    }

    [Fact]
    public void ExtractRegionHierarchy_CountryStateCity()
    {
        var key = SecurityAnalysisHeuristics.ExtractRegionHierarchy("US|CA|SanFrancisco", out var display);
        Assert.Equal("US/CA", key);
        Assert.Equal("US/CA/SanFrancisco", display);
    }
}
