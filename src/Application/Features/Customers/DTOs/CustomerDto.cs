// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
namespace CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

[Description("Customers")]
public class CustomerDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Description")]
    public string? Description {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
        }
    }
}

