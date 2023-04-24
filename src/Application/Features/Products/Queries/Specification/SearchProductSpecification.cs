using CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries;

public class SearchProductSpecification : Specification<Product>
{
    public SearchProductSpecification(ProductsWithPaginationQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            And(x => x.Name!.Contains(query.Keyword) || x.Description!.Contains(query.Keyword) || x.Brand!.Contains(query.Keyword));
        }
        if (!string.IsNullOrEmpty(query.Name))
        {
            And(x => x.Name!.Contains(query.Name));
        }
        if (!string.IsNullOrEmpty(query.Unit))
        {
            And(x => x.Unit == query.Unit);
        }
        if (!string.IsNullOrEmpty(query.Brand))
        {
            And(x => x.Brand == query.Brand);
        }
        if (query.Price is not null)
        {
            And(x => x.Price >= query.Price.Min && x.Price <= query.Price.Max);
        }

    }
}

public enum ProductListView
{
    [Description("All")]
    All,
    [Description("My Products")]
    My,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}
public class SearchProductsWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var end30 = Convert.ToDateTime(today.AddDays(30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        //var currentUser = filterProperty.CurrentUser;
        var listview = (ProductListView)value;
        return listview switch
        {
            ProductListView.All => expressionBody,
            //ProductListView.My=>  Expression.Equal(Expression.Property(expressionBody, "CreatedBy"),  Expression.Constant(currentUser?.UserId)),
            ProductListView.CreatedToday => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"),
                                                                          Expression.Constant(start, typeof(DateTime?)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"),
                                                     Expression.Constant(end, typeof(DateTime?))),
                                                     CombineType.And),
            ProductListView.Created30Days => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"),
                                             Expression.Constant(start, typeof(DateTime?)))
                                             .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"),
                                                     Expression.Constant(end30, typeof(DateTime?))),
                                                     CombineType.And),
            _ => expressionBody
        };
    }
}