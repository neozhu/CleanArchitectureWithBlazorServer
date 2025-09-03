// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

/// <summary>
/// Represents the response of a send mail operation
/// </summary>
public class SendResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the email was sent successfully
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// Gets or sets the list of error messages if any
    /// </summary>
    public IList<string> ErrorMessages { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets additional data related to the send operation
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Creates a successful send response
    /// </summary>
    /// <returns>A successful SendResponse instance</returns>
    public static SendResponse Success() => new() { Successful = true };

    /// <summary>
    /// Creates a successful send response with data
    /// </summary>
    /// <param name="data">The data to include</param>
    /// <returns>A successful SendResponse instance with data</returns>
    public static SendResponse Success(object data) => new() { Successful = true, Data = data };

    /// <summary>
    /// Creates a failed send response with error messages
    /// </summary>
    /// <param name="errors">The error messages</param>
    /// <returns>A failed SendResponse instance</returns>
    public static SendResponse Failure(params string[] errors) => new() 
    { 
        Successful = false, 
        ErrorMessages = errors.ToList() 
    };

    /// <summary>
    /// Creates a failed send response with error messages
    /// </summary>
    /// <param name="errors">The error messages</param>
    /// <returns>A failed SendResponse instance</returns>
    public static SendResponse Failure(IEnumerable<string> errors) => new() 
    { 
        Successful = false, 
        ErrorMessages = errors.ToList() 
    };
}
