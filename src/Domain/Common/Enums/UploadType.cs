// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Common.Enums;

public enum UploadType : byte
{
    [Display(Name ="Products")] Product,
    [Display(Name = "ProfilePictures")] ProfilePicture,
    [Display(Name = "Documents")] Document,
    [Display(Name = "Images")] Image,
}
