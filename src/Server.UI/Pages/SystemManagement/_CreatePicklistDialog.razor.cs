using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement;
public partial class _CreatePicklistDialog
{
    private MudForm? _form = default!;
    private bool _saving = false;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private AddEditKeyValueCommandValidator modelValidator = new AddEditKeyValueCommandValidator();
    [EditorRequired][Parameter] public AddEditKeyValueCommand model { get; set; } = default!;
    [Inject]
    private IMediator _mediator { get; set; } = default!;

    private async Task Submit()
    {
        try
        {
            _saving = true;
            await _form!.Validate().ConfigureAwait(false);
            if (!_form!.IsValid)
                return;
            var result = await _mediator.Send(model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, MudBlazor.Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, MudBlazor.Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}