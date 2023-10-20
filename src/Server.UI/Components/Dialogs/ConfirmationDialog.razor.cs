namespace CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
public partial class ConfirmationDialog
{
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public string? ContentText { get; set; }

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog.Cancel();
}