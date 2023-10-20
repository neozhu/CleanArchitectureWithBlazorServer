using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using Size = SixLabors.ImageSharp.Size;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Products;
public partial class _ProductFormDialog
{
    private MudForm? _form;
    private bool _saving;
    private bool _saveingnew;

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private IUploadService UploadService { get; set; } = default!;

    [Inject]
    private IMediator Mediator { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public AddEditProductCommand Model { get; set; } = default!;

    [Parameter]
    public Action? Refresh { get; set; }

    private const long MaxAllowedSize = 3145728;
    private bool _uploading;

    private async Task DeleteImage(ProductImage picture)
    {
        if (Model.Pictures != null)
        {
            var parameters = new DialogParameters<ConfirmationDialog>
        {
            { x=>x.ContentText, $"{L["Are you sure you want to erase this image?"]}" }
        };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<ConfirmationDialog>($"{L["Erase imatge"]}", parameters, options);
            var state = await dialog.Result;

            if (!state.Canceled)
            {
                Model.Pictures.Remove(picture);
            }
        }
    }

    private async Task PreviewImage(string url, IEnumerable<ProductImage> images)
    {
        await JS.InvokeVoidAsync("previewImage", url, images.Select(x => x.Url).ToArray());
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            _uploading = true;
            var list = new List<ProductImage>();
            foreach (var file in e.GetMultipleFiles())
            {
                try
                {
                    var filestream = file.OpenReadStream(GlobalVariable.MaxAllowedSize);
                    var imgStream = new MemoryStream();
                    await filestream.CopyToAsync(imgStream);
                    imgStream.Position = 0;
                    using (var outStream = new MemoryStream())
                    {
                        using (var image = Image.Load(imgStream))
                        {
                            image.Mutate(
                                i => i.Resize(new ResizeOptions { Mode = ResizeMode.Crop, Size = new Size(640, 320) }));
                            image.Save(outStream, PngFormat.Instance);
                            var filename = file.Name;
                            var fi = new FileInfo(filename);
                            var ext = fi.Extension;
                            var result = await UploadService.UploadAsync(new UploadRequest(Guid.NewGuid() + ext, UploadType.Product, outStream.ToArray()));
                            list.Add(new ProductImage { Name = filename, Size = outStream.Length, Url = result });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"{ex.Message}", Severity.Error);
                }
            }
            Snackbar.Add(L["Upload pictures successfully"], Severity.Info);

            if (Model.Pictures is null)
                Model.Pictures = list;
            else
                Model.Pictures.AddRange(list);
        }
        finally
        {
            _uploading = false;
        }
    }

    private async Task Submit()
    {
        try
        {
            _saving = true;
            await _form!.Validate().ConfigureAwait(false);

            if (!_form!.IsValid)
                return;

            var result = await Mediator.Send(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
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
            _saveingnew = true;
            await _form!.Validate().ConfigureAwait(false);
            if (!_form!.IsValid)
                return;
            var result = await Mediator.Send(Model);
            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
                Refresh?.Invoke();
                Model = new AddEditProductCommand();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saveingnew = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

}