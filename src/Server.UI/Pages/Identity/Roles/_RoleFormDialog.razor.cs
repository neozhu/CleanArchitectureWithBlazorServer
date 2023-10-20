using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Roles;
public partial class _RoleFormDialog
{
    private MudForm? _form;
    private readonly RoleDtoValidator _modelValidator = new();

    [EditorRequired]
    [Parameter]
    public ApplicationRoleDto Model { get; set; } = default!;

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private async Task Submit()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

}