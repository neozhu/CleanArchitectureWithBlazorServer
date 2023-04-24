// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UploadRequest
{
    public UploadRequest(string fileName,  UploadType uploadType, byte[] data)
    {
        FileName = fileName;
        UploadType = uploadType;
        Data = data;
    }

    public string FileName { get; set; }
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; }
    public byte[] Data { get; set; }
}
