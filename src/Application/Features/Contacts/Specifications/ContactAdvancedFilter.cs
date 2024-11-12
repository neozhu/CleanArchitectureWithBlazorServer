﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-11-12
//     Last Modified: 2024-11-12
//     Description: 
//       Defines the available views for filtering contacts and provides advanced 
//       filtering options for contact lists. This includes pagination and various 
//       filters such as view types and user-specific filters.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the Contact list.
/// </summary>
public enum ContactListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}
/// <summary>
/// A class for applying advanced filtering options to Contact lists.
/// </summary>
public class ContactAdvancedFilter: PaginationFilter
{
    public TimeSpan LocalTimezoneOffset { get; set; }
    public ContactListView ListView { get; set; } = ContactListView.All;
    public UserProfile? CurrentUser { get; set; }
}