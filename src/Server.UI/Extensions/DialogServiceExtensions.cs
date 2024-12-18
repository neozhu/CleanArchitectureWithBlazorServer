
namespace CleanArchitecture.Blazor.Server.UI.Extensions;

public static class DialogServiceExtensions
{
    public static async Task ShowEditFormDialog<TDialog, TModel>(
        this IDialogService dialogService,
        string title,
        TModel model,
        Action<DialogOptions>? configureOptions = null,
        Func<Task>? OnSuccess = null,
        Func<Task>? OnFail = null)
        where TDialog : ComponentBase
    {
        var parameters = new DialogParameters
        {
            { "Model", model }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        configureOptions?.Invoke(options);

        var dialog = await dialogService.ShowAsync<TDialog>(title, parameters, options);

        var state = await dialog.Result;

        if (state != null)
        {
            if (!state.Canceled && OnSuccess != null)
            {
                await OnSuccess.Invoke();
            }
            else if (state.Canceled && OnFail != null)
            {
                await OnFail.Invoke();
            }
        }
    }

    public static async Task ShowEditFormDialog<TDialog>(
    this IDialogService dialogService,
    string title,
    int id,
    Func<Task>? OnSuccess = null,
    Func<Task>? OnFail = null)
    where TDialog : ComponentBase
    {

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var parameters = new DialogParameters
        {
            { "ProductId", id }
        };

        var dialog = await dialogService.ShowAsync<TDialog>(title, parameters ,options);

        var state = await dialog.Result;

        if (state != null)
        {
            if (!state.Canceled && OnSuccess != null)
            {
                await OnSuccess.Invoke();
            }
            else if (state.Canceled && OnFail != null)
            {
                await OnFail.Invoke();
            }
        }
    }

}
