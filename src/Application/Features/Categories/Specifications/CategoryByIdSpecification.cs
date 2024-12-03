

namespace CleanArchitecture.Blazor.Application.Features.Categories.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering Categories by their ID.
/// </summary>
public class CategoryByIdSpecification : Specification<Category>
{
    public CategoryByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}