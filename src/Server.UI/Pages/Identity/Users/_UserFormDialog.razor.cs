using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Users;
public partial class _UserFormDialog
{
    private _UserForm? _userForm = default!;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter]
    public ApplicationUserDto Model { get; set; } = default!;

    protected async Task Submit()
    {
        await _userForm!.Submit();
    }

    private void Cancel() => MudDialog.Cancel();

    protected Task OnFormSubmitHandler(ApplicationUserDto model)
    {
        MudDialog.Close(DialogResult.Ok(model));
        return Task.CompletedTask;
    }
}