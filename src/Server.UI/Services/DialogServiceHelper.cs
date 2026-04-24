using System;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using Mediator;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanArchitecture.Blazor.Server.UI.Services;

/// <summary>
/// Helper class for dialog service operations.
/// </summary>
public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;

    // Predefined default options
    private static readonly DialogOptions DefaultOptions = new()
    { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

    private static readonly DialogOptions SmallOptions = new()
    { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogServiceHelper"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    public DialogServiceHelper(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    /// <summary>
    /// Displays a dialog of the specified type asynchronously.
    /// This is the CORE method containing the logic for showing the dialog and handling results.
    /// </summary>
    /// <typeparam name="TDialog">The type of the dialog component to display.</typeparam>
    /// <param name="title">The title to display in the dialog header.</param>
    /// <param name="onConfirm">A callback invoked with the dialog result when the user confirms.</param>
    /// <param name="configureParameters">An optional action to configure the parameters via lambda.</param>
    /// <param name="options">Optional dialog display options.</param>
    /// <param name="onCancel">An optional callback invoked if the dialog is canceled.</param>
    public async Task ShowDialogAsync<TDialog>(
        string title,
        Func<DialogResult, Task> onConfirm,
        Action<DialogParameters<TDialog>>? configureParameters = null,
        DialogOptions? options = null,
        Func<Task>? onCancel = null) where TDialog : IComponent
    {
        // 1. Build parameters internally
        var parameters = new DialogParameters<TDialog>();
        configureParameters?.Invoke(parameters);

        // 2. Set options
        var finalOptions = options ?? DefaultOptions;

        // 3. Show dialog
        var dialog = await _dialogService.ShowAsync<TDialog>(title, parameters, finalOptions);
        var result = await dialog.Result;

        // 4. Handle result
        if (result is not null && !result.Canceled)
        {
            await onConfirm.Invoke(result);
        }
        else
        {
            if (onCancel != null)
            {
                await onCancel.Invoke();
            }
        }
    }

    /// <summary>
    /// Displays a form dialog of the specified component type.
    /// Essentially a wrapper around ShowDialogAsync with type constraints for TCommand.
    /// </summary>
    public async Task ShowFormDialogAsync<TDialog, TCommand>(
        string title,
        Func<DialogResult, Task> onDialogResult,
        Action<DialogParameters<TDialog>>? configureParameters = null,
        DialogOptions? options = null,
        Func<Task>? onCancel = null)
        where TDialog : ComponentBase
        where TCommand : IRequest<Result>
    {
        // Delegate directly to the core method
        await ShowDialogAsync<TDialog>(
            title,
            onDialogResult,
            configureParameters,
            options ?? DefaultOptions,
            onCancel
        );
    }

    /// <summary>
    /// Shows a delete confirmation dialog with a Generic Command.
    /// </summary>
    public async Task ShowDeleteConfirmationDialogAsync<TCommand>(
        TCommand command,
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null)
        where TCommand : IRequest<Result>
    {
        // Adapt: Wrap Func<Task> into Func<DialogResult, Task>
        Func<DialogResult, Task> confirmWrapper = _ => onConfirm();

        // Delegate to core method, configuring parameters via Lambda
        await ShowDialogAsync<DeleteConfirmation>(
            title,
            confirmWrapper,
            parameters =>
            {
                parameters.Add(x => x.ContentText, contentText);
                parameters.Add(x => x.Command, command);
            },
            SmallOptions,
            onCancel
        );
    }

    /// <summary>
    /// Shows a simple confirmation dialog.
    /// </summary>
    public async Task ShowConfirmationDialogAsync(
        string title,
        string contentText,
        Func<Task> onConfirm,
        Func<Task>? onCancel = null)
    {
        // Adapt: Wrap Func<Task> into Func<DialogResult, Task>
        Func<DialogResult, Task> confirmWrapper = _ => onConfirm();

        // Delegate to core method, configuring parameters via Lambda
        await ShowDialogAsync<ConfirmationDialog>(
            title,
            confirmWrapper,
            parameters => parameters.Add(x => x.ContentText, contentText),
            SmallOptions,
            onCancel
        );
    }
}
