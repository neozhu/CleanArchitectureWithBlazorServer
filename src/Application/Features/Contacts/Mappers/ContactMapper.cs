using CleanArchitecture.Blazor.Application.Features.Contacts.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class ContactMapper
{
    public static partial ContactDto ToDto(Contact contact);
    public static partial Contact Map(ContactDto dto);
    public static partial UpdateContactCommand ToUpdateCommand(ContactDto dto);
    public static partial Contact Map(AddEditContactCommand command);
    public static partial Contact Map(CreateContactCommand command);
    public static partial void MapTo(UpdateContactCommand command, Contact contact);
    public static partial void MapTo(AddEditContactCommand command, Contact contact);
    public static partial IQueryable<ContactDto> ProjectTo(this IQueryable<Contact> q);
}
