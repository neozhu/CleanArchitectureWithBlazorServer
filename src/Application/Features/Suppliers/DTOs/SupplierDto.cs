namespace CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;

[Description("Suppliers")]
public record SupplierDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Name")]
    public string Name {get;set;} 
    [Description("Address")]
    public string? Address {get;set;} 
    [Description("Phone")]
    public string? Phone {get;set;} 
    [Description("Email")]
    public string? Email {get;set;} 
    [Description("Vat")]
    public string? VAT {get;set;} 
    [Description("Country")]
    public string? Country {get;set;} 
    [Description("Created at")]
    public DateTime CreatedAt {get;set;} 


}

