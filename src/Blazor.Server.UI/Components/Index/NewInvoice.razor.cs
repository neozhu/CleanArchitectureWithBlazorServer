using MudBlazor;
using MudBlazor.Utilities;
using MudDemo.Server.Models.Invoice;

namespace MudDemo.Server.Components.Index;

public partial class NewInvoice : MudComponentBase
{
    private readonly List<InvoiceModel> _invoices = new()
    {
        new InvoiceModel
        {
            InvoiceId = Guid.NewGuid(),
            Category = "Android",
            Price = 16.99,
            Status = InvoiceStatus.Paid
        },
        new InvoiceModel
        {
            InvoiceId = Guid.NewGuid(),
            Category = "Windows",
            Price = 35.99,
            Status = InvoiceStatus.InProgress
        },
        new InvoiceModel
        {
            InvoiceId = Guid.NewGuid(),
            Category = "Mac",
            Price = 14.99,
            Status = InvoiceStatus.OutOfDate
        },
        new InvoiceModel
        {
            InvoiceId = Guid.NewGuid(),
            Category = "Windows",
            Price = 19.99,
            Status = InvoiceStatus.InProgress
        },
        new InvoiceModel
        {
            InvoiceId = Guid.NewGuid(),
            Category = "Windows",
            Price = 99.99,
            Status = InvoiceStatus.Paid
        }
    };

    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();
}