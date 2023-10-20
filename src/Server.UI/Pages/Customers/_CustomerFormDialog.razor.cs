using CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Customers;
public partial class _CustomerFormDialog
{
    private MudForm? _form;
    private bool _saving = false;
    private bool _savingnew = false;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private AddEditCustomerCommandValidator _modelValidator = new();
    [EditorRequired][Parameter] public AddEditCustomerCommand model { get; set; } = null!;
    [Inject] private IMediator _mediator { get; set; } = default!;

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

    private async Task SaveAndNew()
    {
        try
        {
            _savingnew = true;
            await _form!.Validate().ConfigureAwait(false);
            if (!_form!.IsValid)
                return;
            var result = await _mediator.Send(model);
            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.SaveSuccess, MudBlazor.Severity.Info);
                await Task.Delay(300);
                model = new AddEditCustomerCommand() { };
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, MudBlazor.Severity.Error);
            }
        }
        finally
        {
            _savingnew = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}