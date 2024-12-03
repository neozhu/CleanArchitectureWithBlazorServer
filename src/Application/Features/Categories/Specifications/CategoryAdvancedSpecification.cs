

namespace CleanArchitecture.Blazor.Application.Features.Categories.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of Categories.
/// </summary>
public class CategoryAdvancedSpecification : Specification<Category>
{
    public CategoryAdvancedSpecification(CategoryAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(CategoryListView.TODAY.ToString(), filter.LocalTimezoneOffset);
        var last30daysrange = today.GetDateRange(CategoryListView.LAST_30_DAYS.ToString(),filter.LocalTimezoneOffset);

        Query.Where(q => q.Name != null)
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == CategoryListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == CategoryListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == CategoryListView.LAST_30_DAYS);
       
    }
}
