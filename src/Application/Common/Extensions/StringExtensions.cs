// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Blazor.Infrastructure.Common.Extensions;

public static class StringExtensions
{
    public static string ToMD5(this string input)
    {
        using (var md5 = MD5.Create())
        {
            var encoding = Encoding.ASCII;
            var data = encoding.GetBytes(input);

            Span<byte> hashBytes = stackalloc byte[16];
            md5.TryComputeHash(data, hashBytes, out var written);
            if (written != hashBytes.Length)
                throw new OverflowException();


            Span<char> stringBuffer = stackalloc char[32];
            for (var i = 0; i < hashBytes.Length; i++) hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
            return new string(stringBuffer);
        }
    }

    static char[] trimChars = { ' ', '\t', '\n', '\r' };
    public static string? TrimSelf(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        input = TrimSelf(new StringBuilder(input));
        return input;
    }
    public static string? TrimSelf(this StringBuilder input)
    {
        if (input == null)
            return null;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            if (trimChars.Contains(input[i]))
            {
                input.Remove(i, 1);
            }
        }
        return input.ToString();
    }
    public static string? TrimResult(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        char[] trimChars = { ' ', '\t', '\n', '\r' };
        // Create a StringBuilder to build the trimmed string
        var trimmed = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            if (!trimChars.Contains(c))
            {
                trimmed.Append(c);
            }
        }

        return trimmed.ToString();
    }
    public static bool IsNullOrEmpty(this string? input)
    {
        return input == null ? false : string.IsNullOrEmpty(input.TrimResult());
    }
    public static bool IsNullOrEmptyAndTrimSelf(this string? input)
    {
        return string.IsNullOrEmpty(input.TrimSelf());
    }
}