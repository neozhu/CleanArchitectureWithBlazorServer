using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;

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
    /// Shows a form dialog with the specified model and handles actions on completion.
    /// </summary>
    /// <typeparam name="TDialog">The type of dialog component to show.</typeparam>
    /// <typeparam name="TModel">The type of model to pass to the dialog.</typeparam>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="model">The model to pass to the dialog.</param>
    /// <param name="onDialogResult">Action to perform when dialog returns a non-cancelled result.</param>
    /// <param name="parameterName">The name of the parameter in the dialog component (default is "_model").</param>
    /// <param name="options">Dialog options (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShowFormDialogAsync<TDialog, TModel>(
        string title,
        TModel model,
        Func<Task> onDialogResult,
        string parameterName = "_model",
        DialogOptions? options = null) where TDialog : ComponentBase
    {
        var parameters = new DialogParameters
        {
            { parameterName, model }
        };

        var dialogOptions = options ?? new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<TDialog>(title, parameters, dialogOptions);
        var result = await dialog.Result;

        if (result != null && !result.Canceled)
        {
            await onDialogResult();
        }
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
    public async Task ShowDeleteConfirmationDialogAsync(
        object command,
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), contentText },
            { nameof(DeleteConfirmation.Command), command }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<DeleteConfirmation>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
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
    public async Task ShowConfirmationDialogAsync(
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), contentText }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }
}
