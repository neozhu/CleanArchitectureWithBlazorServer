// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

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
}