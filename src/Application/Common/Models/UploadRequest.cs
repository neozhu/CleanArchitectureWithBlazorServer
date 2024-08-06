// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UploadRequest
{
    public UploadRequest(string fileName, UploadType uploadType, byte[] data, bool overwrite=false)
    {
        FileName = fileName;
        UploadType = uploadType;
        Data = data;
        Overwrite = overwrite;
    }

    public string FileName { get; set; }
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; }
    public bool Overwrite { get; set; }
    public byte[] Data { get; set; }
}