

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;
public class CrmProductCategory : BaseAuditableEntity
{
    public string CategoryName { get; set; }
}