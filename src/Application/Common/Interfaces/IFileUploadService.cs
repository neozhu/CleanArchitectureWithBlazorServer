// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for services that handle file upload and removal operations asynchronously.
/// </summary>
/// <remarks>Implementations of this interface should perform necessary validation and error handling for file
/// operations. Methods are asynchronous and may involve I/O operations such as saving files to disk or cloud storage.
/// Callers should ensure that input parameters meet any required constraints and handle exceptions as
/// appropriate.</remarks>
public interface IFileUploadService
{
    /// <summary>
    /// Uploads a file asynchronously using the specified upload request.
    /// </summary>
    /// <param name="request">The upload request that contains the file data and associated metadata required for the upload operation. Must
    /// not be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{UploadedFileInfo}"/> indicating the
    /// outcome of the upload.</returns>
    Task<Result<UploadedFileInfo>> UploadAsync(UploadRequest request);
    /// <summary>
    /// Removes the resource identified by the specified URI asynchronously.
    /// </summary>
    /// <remarks>This method may throw exceptions if the URI is invalid or if the resource cannot be
    /// found.</remarks>
    /// <param name="uri">The URI of the resource to be removed. This must be a valid URI string.</param>
    /// <returns>A task that represents the asynchronous operation of removing the resource.</returns>
    Task RemoveAsync(string uri);
}