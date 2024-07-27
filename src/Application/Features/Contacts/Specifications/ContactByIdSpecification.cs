namespace CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering Contacts by their ID.
/// </summary>
public class ContactByIdSpecification : Specification<Contact>
{
    public ContactByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}