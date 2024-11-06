using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Blazor.Application.Features.Products.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class ProductMapper
{
    public static partial ProductDto ToDto(Product product);
    public static partial Product Map(ProductDto dto);
    public static partial Product Map(AddEditProductCommand command);
    public static partial void MapTo(AddEditProductCommand command, Product product);
    [MapperIgnoreSource(nameof(ProductDto.Id))]
    public static partial AddEditProductCommand Clone(ProductDto dto);
    public static partial AddEditProductCommand ToEditCommand(ProductDto dto);
    public static partial IQueryable<ProductDto> ProjectTo(this IQueryable<Product> q);
}
