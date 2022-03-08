// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Blazor.Server.UI.Models.Application;

public class ApplicationModel
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public double Price { get; set; }
    public int ReviewScore { get; set; }
    public int NumberOfReview { get; set; }
}