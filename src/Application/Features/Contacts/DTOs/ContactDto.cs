// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;

[Description("Contacts")]
public class ContactDto
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Name")]
    public string Name { get; set; } = String.Empty;
    [Description("Description")]
    public string? Description { get; set; }
    [Description("Email")]
    public string? Email { get; set; }
    [Description("Phone Number")]
    public string? PhoneNumber { get; set; }
    [Description("Country")]
    public string? Country { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Contact, ContactDto>().ReverseMap();
        }
    }
}

