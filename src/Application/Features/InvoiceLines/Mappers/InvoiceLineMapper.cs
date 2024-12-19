
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class InvoiceLineMapper
{
    public static partial InvoiceLineDto ToDto(InvoiceLine source);
    public static partial InvoiceLine FromDto(InvoiceLineDto dto);
    public static partial InvoiceLine FromEditCommand(AddEditInvoiceLineCommand command);
    public static partial InvoiceLine FromCreateCommand(CreateInvoiceLineCommand command);
    public static partial UpdateInvoiceLineCommand ToUpdateCommand(InvoiceLineDto dto);
    public static partial AddEditInvoiceLineCommand CloneFromDto(InvoiceLineDto dto);
    public static partial void ApplyChangesFrom(UpdateInvoiceLineCommand source, InvoiceLine target);
    public static partial void ApplyChangesFrom(AddEditInvoiceLineCommand source, InvoiceLine target);
    public static partial IQueryable<InvoiceLineDto> ProjectTo(this IQueryable<InvoiceLine> q);
}

