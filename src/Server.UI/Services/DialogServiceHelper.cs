using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using MediatR;

namespace CleanArchitecture.Blazor.Server.UI.Services;

/// <summary>
/// Helper class for dialog service operations.
/// </summary>
public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;


    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceHelper"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    public DialogServiceHelper(IDialogService dialogService)
    {
        _dialogService = dialogService;
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
    public async Task ShowDeleteConfirmationDialogAsync(IRequest<Result<int>> command, string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
            {
                { nameof(DeleteConfirmation.ContentText), contentText },
                { nameof(DeleteConfirmation.Command), command }
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<DeleteConfirmation>(title, parameters, options).ConfigureAwait(false);
        var result = await dialog.Result.ConfigureAwait(false);
        if (result is not null && !result.Canceled)
        {
            await onConfirm().ConfigureAwait(false);
        }
        else if (onCancel != null)
        {
            await onCancel().ConfigureAwait(false);
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
    public async Task ShowConfirmationDialogAsync(string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
            {
                { nameof(ConfirmationDialog.ContentText), contentText }
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>(title, parameters, options).ConfigureAwait(false);
        var result = await dialog.Result.ConfigureAwait(false);

        if (result is not null && !result.Canceled)
        {
            await onConfirm().ConfigureAwait(false);
        }
        else if (onCancel != null)
        {
            await onCancel().ConfigureAwait(false);
        }
    }
}
