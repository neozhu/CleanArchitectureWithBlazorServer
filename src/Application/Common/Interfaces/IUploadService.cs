// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using SixLabors.ImageSharp.Processing;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IUploadService
{
    Task<string> UploadAsync(UploadRequest request);
    void Remove(string filename);

    Task<string> UploadImageAsync(Stream imageStream,  UploadType uploadType, ResizeOptions? resizeOptions = null, string? fileName=null);
}