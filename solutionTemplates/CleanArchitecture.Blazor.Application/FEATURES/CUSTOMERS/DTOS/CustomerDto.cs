// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.DTOs;


public class CustomerDto:IMapFrom<Customer>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Customer, CustomerDto>().ReverseMap();
    }
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

}

