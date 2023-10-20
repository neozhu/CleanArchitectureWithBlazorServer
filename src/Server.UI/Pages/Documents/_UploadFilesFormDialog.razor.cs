using CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Documents.Commands.Upload;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Server.UI.Services.JsInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Documents;
public partial class _UploadFilesFormDialog
{

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [EditorRequired][Parameter] public AddEditDocumentCommand Model { get; set; } = default!;

    private bool _processing;
    private bool _uploading;
    [Inject]
    private IJSRuntime Js { get; set; } = default!;
    [Inject]
    private ISender Mediator { get; set; } = default!;
    [Inject] private IUploadService UploadService { get; set; } = default!;

    private List<FileUploadProgress> _uploadedFiles = new();
    private async void LoadFiles(IReadOnlyList<IBrowserFile> files)
    {
        try
        {
            _uploading = true;
            var startIndex = _uploadedFiles.Count;
            // Add all files to the UI
            _uploadedFiles.AddRange(files.Select(file => new FileUploadProgress(file.Name, file.Size, file)).ToList());


            // We don't want to refresh the UI too frequently,
            // So, we use a timer to update the UI every few hundred milliseconds
            await using var timer = new Timer(_ => InvokeAsync(() => StateHasChanged()));
            timer.Change(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));

            // Upload files
            byte[] buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                foreach (var file in files)
                {
                    using (var stream = file.OpenReadStream(GlobalVariable.MaxAllowedSize))
                    {
                        while (true)
                        {
                            var read = await stream.ReadAsync(buffer);
                            if (read == 0)
                            {
                                break; // Exit loop if no more data to read
                            }

                            _uploadedFiles[startIndex].UploadedBytes += read;

                            // TODO Do something with the file chunk, such as save it
                            // to a database or a local file system
                            var readData = buffer.AsMemory().Slice(0, read);
                        }
                    }

                    startIndex++;
                }
            }
            finally
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(buffer);

                // AddOrUpdate the UI with the final progress
                StateHasChanged();
            }
        }
        finally
        {
            _uploading = false;
            StateHasChanged();
        }
    }

    // Use the Meziantou.Framework.ByteSize NuGet package.
    // You could also use Humanizer
    private string FormatBytes(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    private async Task Submit()
    {
        try
        {
            _processing = true;
            if (_uploadedFiles.Any())
            {
                var list = new List<UploadRequest>();
                foreach (var uploaded in _uploadedFiles)
                {
                    try
                    {
                        var filestream = uploaded.File.OpenReadStream(GlobalVariable.MaxAllowedSize);
                        var imgStream = new MemoryStream();
                        await filestream.CopyToAsync(imgStream);
                        imgStream.Position = 0;
                        using (var outStream = new MemoryStream())
                        {
                            using (var image = SixLabors.ImageSharp.Image.Load(imgStream))
                            {
                                var scale = 0m;
                                if (image.Width > 1600)
                                {
                                    scale = 0.3m;
                                }
                                else if (image.Width <= 1600 && image.Width > 800)
                                {
                                    scale = 0.5m;
                                }
                                else
                                {
                                    scale = 0.9m;
                                }
                                image.Mutate(
                                   i => i.AutoOrient().Resize(Convert.ToInt32(image.Width * scale), 0));
                                image.Save(outStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
                            }
                            var request = new UploadRequest(uploaded.FileName, UploadType.Document, outStream.ToArray());
                            list.Add(request);
                        }
                    }
                    catch (Exception e)
                    {
                        Snackbar.Add($"{e.Message}", MudBlazor.Severity.Error);
                    }
                }
                var uploadCommand = new UploadDocumentCommand(list);
                await Mediator.Send(uploadCommand);
                await Clear();
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        finally
        {
            _processing = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Clear()
    {
        await new InputClear(Js).Clear("fileInput");
        _uploadedFiles = new();
    }

    private record FileUploadProgress(string FileName, long Size, IBrowserFile File)
    {
        public long UploadedBytes { get; set; }
        public double UploadedPercentage => (double)UploadedBytes / (double)Size * 100d;
    }
}