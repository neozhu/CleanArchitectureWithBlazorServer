namespace CleanArchitecture.Blazor.Domain.Features.Products;
public class ProductImage
{
    public required string Name { get; set; }
    public decimal Size { get; set; }
    public required string Url { get; set; }
}