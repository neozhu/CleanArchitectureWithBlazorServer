

using CleanArchitecture.Blazor.Application.Features.Categories.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Categories.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.Categories.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.Categories.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class CategoryMapper
{
    public static partial CategoryDto ToDto(Category source);
    public static partial Category FromDto(CategoryDto dto);
    public static partial Category FromEditCommand(AddEditCategoryCommand command);
    public static partial Category FromCreateCommand(CreateCategoryCommand command);
    public static partial UpdateCategoryCommand ToUpdateCommand(CategoryDto dto);
    public static partial AddEditCategoryCommand CloneFromDto(CategoryDto dto);
    public static partial void ApplyChangesFrom(UpdateCategoryCommand source, Category target);
    public static partial void ApplyChangesFrom(AddEditCategoryCommand source, Category target);
    public static partial IQueryable<CategoryDto> ProjectTo(this IQueryable<Category> q);
}

