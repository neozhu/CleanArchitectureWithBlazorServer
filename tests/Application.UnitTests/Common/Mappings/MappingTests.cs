using System;
using System.Reflection;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;
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
    [TestCase(typeof(Document), typeof(DocumentDto))]
    [TestCase(typeof(KeyValue), typeof(KeyValueDto))]
    [TestCase(typeof(Product), typeof(ProductDto))]
    [TestCase(typeof(ProductDto), typeof(Product))]
    [TestCase(typeof(KeyValueDto), typeof(KeyValue))]
    [TestCase(typeof(DocumentDto), typeof(Document))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type);

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}

