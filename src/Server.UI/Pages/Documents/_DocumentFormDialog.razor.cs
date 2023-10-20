using CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Documents;
public partial class _DocumentFormDialog
{
    private MudForm? _form = default!;
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private readonly AddEditDocumentCommandValidator _modelValidator = new AddEditDocumentCommandValidator();
    [EditorRequired][Parameter] public AddEditDocumentCommand Model { get; set; } = default!;

    private const long MaxAllowedSize = 3145728;

    private async Task Submit()
    {
        await _form!.Validate();
        if (_form!.IsValid)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

    }

    private void Cancel() => MudDialog.Cancel();


}