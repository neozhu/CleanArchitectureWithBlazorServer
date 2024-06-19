using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using MediatR;

namespace CleanArchitecture.Blazor.Server.UI.Services;

/// <summary>
/// Helper class for dialog service operations.
/// </summary>
public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceHelper"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="snackbar">The snackbar.</param>
    public DialogServiceHelper(IDialogService dialogService, ISnackbar snackbar)
    {
        _dialogService = dialogService;
        _snackbar = snackbar;
    }

    /// <summary>
    /// Shows a delete confirmation dialog.
    /// </summary>
    /// <param name="command">The command to execute on confirmation.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="contentText">The content text of the dialog.</param>
    /// <param name="onConfirm">The action to perform on confirmation.</param>
    /// <param name="onCancel">The action to perform on cancellation (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShowDeleteConfirmationDialog(IRequest<Result<int>> command, string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
            {
                { nameof(DeleteConfirmation.ContentText), contentText },
                { nameof(DeleteConfirmation.Command), command }
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = _dialogService.Show<DeleteConfirmation>(title, parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }

    /// <summary>
    /// Shows a confirmation dialog.
    /// </summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="contentText">The content text of the dialog.</param>
    /// <param name="onConfirm">The action to perform on confirmation.</param>
    /// <param name="onCancel">The action to perform on cancellation (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShowConfirmationDialog(string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
            {
                { nameof(ConfirmationDialog.ContentText), contentText }
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = _dialogService.Show<ConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }
}
