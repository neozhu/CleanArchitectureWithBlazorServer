using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CleanArchitecture.Blazor.Application.Common.Mappings;
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
using Mapster;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly TypeAdapterConfig _configuration;
    private readonly MapsterObjectMapper _mapper;
    private readonly MethodInfo _mapMethod;

    public MappingTests()
    {
        _configuration = MapsterConfiguration.Create();
        _mapper = new MapsterObjectMapper(_configuration);
        _mapMethod = typeof(MapsterObjectMapper).GetMethods()
            .Single(method => method.Name == nameof(MapsterObjectMapper.Map)
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 1);
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        Assert.DoesNotThrow(() => _configuration.Compile());
    }

    [Test]
    [TestCase(typeof(Document), typeof(DocumentDto), true)]
    [TestCase(typeof(Tenant), typeof(TenantDto), true)]
    [TestCase(typeof(Product), typeof(ProductDto), true)]
    [TestCase(typeof(Contact), typeof(ContactDto), true)]
    [TestCase(typeof(PicklistSet), typeof(PicklistSetDto), true)]
    [TestCase(typeof(ApplicationUser), typeof(ApplicationUserDto), false)]
    [TestCase(typeof(ApplicationUser), typeof(UserBriefDto), false)]
    [TestCase(typeof(ApplicationRole), typeof(ApplicationRoleDto), false)]
    [TestCase(typeof(SystemLog), typeof(SystemLogDto), false)]
    [TestCase(typeof(AuditTrail), typeof(AuditTrailDto), false)]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination, bool inverseMap = false)
    {
        var instance = GetInstanceOf(source);

        Assert.DoesNotThrow(() => Map(instance, destination));

        if (inverseMap)
        {
            var destinationInstance = GetInstanceOf(destination);
            Assert.DoesNotThrow(() => Map(destinationInstance, source));
        }
    }

    private object Map(object source, Type destinationType)
    {
        return _mapMethod.MakeGenericMethod(destinationType).Invoke(_mapper, new[] { source })!;
    }

    private static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) is not null)
            return Activator.CreateInstance(type)!;

#pragma warning disable SYSLIB0050
        return FormatterServices.GetUninitializedObject(type);
#pragma warning restore SYSLIB0050
    }
}
