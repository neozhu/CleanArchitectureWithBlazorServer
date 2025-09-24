using System;
using System.Reflection;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Mappings;
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));
        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(Document), typeof(DocumentDto), true)]
    [TestCase(typeof(Tenant), typeof(TenantDto), true)]
    [TestCase(typeof(Product), typeof(ProductDto), true)]
    [TestCase(typeof(Contact), typeof(ContactDto), true)]
    [TestCase(typeof(PicklistSet), typeof(PicklistSetDto), true)]
    [TestCase(typeof(ApplicationUser), typeof(ApplicationUserDto), false)]
    [TestCase(typeof(ApplicationRole), typeof(ApplicationRoleDto), false)]
    [TestCase(typeof(SystemLog), typeof(SystemLogDto), false)]
    [TestCase(typeof(AuditTrail), typeof(AuditTrailDto), false)]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination, bool inverseMap = false)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);

        if (inverseMap)
        {
            ShouldSupportMappingFromSourceToDestination(destination, source, false);
        }
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type);

        throw new InvalidOperationException($"Type {type.FullName} does not have a parameterless constructor.");
    }
}
