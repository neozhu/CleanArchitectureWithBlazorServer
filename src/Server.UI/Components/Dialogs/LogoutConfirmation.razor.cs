namespace CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
public partial class LogoutConfirmation
{
    [Parameter] public string? ContentText { get; set; }

    [Parameter] public Color Color { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private Task Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
        Snackbar.Add(@ConstantString.LogoutSuccess, MudBlazor.Severity.Info);
        return Task.CompletedTask;
    }

    private void Cancel() => MudDialog.Cancel();
}