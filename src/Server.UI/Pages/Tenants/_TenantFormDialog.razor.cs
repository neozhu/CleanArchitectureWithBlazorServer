using CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Tenants;
public partial class _TenantFormDialog
{
    private MudForm? _form = default!;
    private bool _saving = false;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private readonly AddEditTenantCommandValidator _modelValidator = new AddEditTenantCommandValidator();
    [EditorRequired][Parameter] public AddEditTenantCommand Model { get; set; } = default!;
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
            var result = await _mediator.Send(Model);

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