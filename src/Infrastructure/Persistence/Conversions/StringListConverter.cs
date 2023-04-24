// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Conversions;

public class StringListConverter : ValueConverter<List<string>, string>
{
    public StringListConverter() : base(v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
        v => JsonSerializer.Deserialize<List<string>>(string.IsNullOrEmpty(v) ? "[]" : v,
            DefaultJsonSerializerOptions.Options) ?? new List<string>()
    )
    {
    }
}