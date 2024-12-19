
using CleanArchitecture.Blazor.Application.Features.Invoices.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;

namespace CleanArchitecture.Blazor.Application.Invoices.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class InvoiceMapper
{
    public static partial InvoiceDto ToDto(Invoice source);
    public static partial Invoice FromDto(InvoiceDto dto);
    public static partial Invoice FromEditCommand(AddEditInvoiceCommand command);
    public static partial Invoice FromCreateCommand(CreateInvoiceCommand command);
    public static partial UpdateInvoiceCommand ToUpdateCommand(InvoiceDto dto);
    public static partial AddEditInvoiceCommand CloneFromDto(InvoiceDto dto);
    public static partial void ApplyChangesFrom(UpdateInvoiceCommand source, Invoice target);
    public static partial void ApplyChangesFrom(AddEditInvoiceCommand source, Invoice target);
    public static partial IQueryable<InvoiceDto> ProjectTo(this IQueryable<Invoice> q);
}

