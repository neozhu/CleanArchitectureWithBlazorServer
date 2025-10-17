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
    /// Displays a dialog form of the specified component type and executes a callback if the dialog is completed
    /// successfully.
    /// </summary>
    /// <remarks>The dialog is shown modally. The onDialogResult callback is only executed if the user
    /// completes the dialog without canceling. If options is null, default dialog options are applied.</remarks>
    /// <typeparam name="TDialog">The type of the dialog component to display. Must inherit from ComponentBase.</typeparam>
    /// <typeparam name="TModel">The type of the model associated with the dialog. This type parameter is not directly used by this method but
    /// may be required for dialog parameterization.</typeparam>
    /// <param name="title">The title to display in the dialog window.</param>
    /// <param name="dialogParamters">The parameters to pass to the dialog component. Provides data or configuration for the dialog instance.</param>
    /// <param name="onDialogResult">A callback to invoke if the dialog is closed without being canceled. The callback is not called if the dialog is
    /// canceled.</param>
    /// <param name="options">Optional dialog display options. If null, default options are used.</param>
    /// <returns>A task that represents the asynchronous operation of displaying the dialog and handling the result.</returns>
    public async Task ShowFormDialogAsync<TDialog, TModel>(
        string title,
        DialogParameters<TDialog> dialogParamters,
        Func<Task> onDialogResult,
        DialogOptions? options=null ) where TDialog : ComponentBase
    {
        

        var dialogOptions = options ?? new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<TDialog>(title, dialogParamters, dialogOptions);
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
        var parameters = new DialogParameters<DeleteConfirmation>
        {
            { x=>x.ContentText, contentText },
            { x=>x.Command, command }
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
        var parameters = new DialogParameters<ConfirmationDialog>
        {
            { x=>x.ContentText, contentText }
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
