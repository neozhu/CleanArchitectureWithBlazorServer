using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Server.UI.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Server.UI.Services;

public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DialogServiceHelper(IDialogService dialogService, ISnackbar snackbar, IStringLocalizer<SharedResource> localizer)
    {
        _dialogService = dialogService;
        _snackbar = snackbar;
        _localizer = localizer;
    }

    public async Task<bool> ShowDeleteConfirmationDialog(IRequest<Result<int>> command, string contentText)
    {
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), contentText },
            { nameof(DeleteConfirmation.Command), command }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = _dialogService.Show<DeleteConfirmation>(_localizer["DeleteConfirmationTitle"], parameters, options);
        var result = await dialog.Result;

        return !result.Canceled && result.Data as bool? == true;
    }

    public async Task<bool> ShowConfirmationDialog(string contentText)
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), contentText }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = _dialogService.Show<ConfirmationDialog>(_localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;

        return !result.Canceled && result.Data as bool? == true;
    }
}
