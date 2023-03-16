using System.Linq.Expressions;
using System.Reflection;
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
        if(query.Price is not null)
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
}
public class SearchProductsWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var listview = (ProductListView)value;
        return listview switch {
            ProductListView.All => expressionBody,
            ProductListView.My=> expressionBody,
            ProductListView.CreatedToday => expressionBody,
            _=> expressionBody
        };
    }
}