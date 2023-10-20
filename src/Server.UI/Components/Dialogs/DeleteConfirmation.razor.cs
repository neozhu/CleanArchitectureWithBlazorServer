namespace CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
public partial class DeleteConfirmation
{
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public string? ContentText { get; set; }
    [EditorRequired]
    [Parameter]
    public IRequest<Result<int>> Command { get; set; } = default!;
    [Inject]
    private IMediator Mediator { get; set; } = default!;

    private async Task Submit()
    {
        var result = await Mediator.Send(Command);
        if (result.Succeeded)
        {
            Snackbar.Add($"{ConstantString.DeleteSuccess}", MudBlazor.Severity.Info);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", MudBlazor.Severity.Error);
        }

    }

    private void Cancel() => MudDialog.Cancel();
}