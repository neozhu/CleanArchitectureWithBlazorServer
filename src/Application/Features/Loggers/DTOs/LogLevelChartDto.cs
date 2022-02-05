// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Logs.DTOs;

public class LogLevelChartDto
{
    public string level { get; set; }
    public int total { get; set; }
}

public class LogTimeLineDto
{
    public DateTime time { get; set; }
    public int total { get; set; }
    public string level { get; set; }
}
