using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Features.KeyValues;
public enum Picklist
{
    [Description("Status")]
    Status,
    [Description("Unit")]
    Unit,
    [Description("Brand")]
    Brand
}
