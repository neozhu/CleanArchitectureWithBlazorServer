namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Users;
public partial class _ResetPasswordDialog
{
    private MudForm? _form = default!;
    private readonly ResetPasswordFormModelValidator _modelValidator = new ResetPasswordFormModelValidator();
    [EditorRequired][Parameter] public ResetPasswordFormModel Model { get; set; } = default!;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private async Task Submit()
    {
        await _form!.Validate();
        if (_form.IsValid)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

    }

    private void Cancel() => MudDialog.Cancel();
}