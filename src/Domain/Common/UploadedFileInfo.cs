// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common;

public class UploadedFileInfo
{
    public required string Name { get; set; }
    public long Size { get; set; }
    public required string Url { get; set; }
}
