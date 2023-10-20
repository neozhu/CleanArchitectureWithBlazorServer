using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using Size = SixLabors.ImageSharp.Size;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Users;
public partial class _UserForm : MudComponentBase
{
    public class CheckItem
    {
        public string Key { get; set; } = string.Empty;
        public bool Value { get; set; }
    }

    private MudForm? _form = default!;
    private List<CheckItem> Roles { get; set; } = new();

    [Inject]
    private IUploadService UploadService { get; set; } = default!;

    [Inject]
    private ITenantService TenantsService { get; set; } = default!;

    [Inject]
    private RoleManager<ApplicationRole> RoleManager { get; set; } = default!;

    private ApplicationUserDtoValidator _modelValidator = new();

    [EditorRequired]
    [Parameter]
    public ApplicationUserDto Model { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public EventCallback<ApplicationUserDto> OnFormSubmit { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var array = await RoleManager.Roles.Select(x => x.Name).ToListAsync();
        foreach (var role in array)
        {
            if (Model.AssignedRoles != null && Model.AssignedRoles.Contains(role))
            {
                Roles.Add(new CheckItem { Key = role!, Value = true });
            }
            else
            {
                Roles.Add(new CheckItem { Key = role!, Value = false });
            }
        }
    }

    private Task TenantSelectChanged(string id)
    {
        var tenant = TenantsService.DataSource.Find(x => x.Id == id);
        Model.TenantId = tenant?.Id;
        Model.TenantName = tenant?.Name;
        return Task.CompletedTask;
    }

    private async Task UploadPhoto(InputFileChangeEventArgs e)
    {
        var filestream = e.File.OpenReadStream(GlobalVariable.MaxAllowedSize);
        var imgStream = new MemoryStream();
        await filestream.CopyToAsync(imgStream);
        imgStream.Position = 0;
        using (var outStream = new MemoryStream())
        {
            using (var image = Image.Load(imgStream))
            {
                image.Mutate(
                    i => i.Resize(new ResizeOptions { Mode = ResizeMode.Crop, Size = new Size(128, 128) }));
                image.Save(outStream, PngFormat.Instance);
                var filename = e.File.Name;
                var fi = new FileInfo(filename);
                var ext = fi.Extension;
                var result = await UploadService.UploadAsync(new UploadRequest(Guid.NewGuid() + ext, UploadType.ProfilePicture, outStream.ToArray()));
                Model.ProfilePictureDataUrl = result;
                //Do your validations here
                Snackbar.Add(ConstantString.UploadSuccess, Severity.Info);
            }
        }
    }

    public async Task Submit()
    {
        if (_form is not null)
        {
            await _form.Validate();
            if (_form.IsValid)
            {
                Model.AssignedRoles = Roles.Where(x => x.Value).Select(x => x.Key).ToArray();
                await OnFormSubmit.InvokeAsync(Model);
            }
        }
    }


}

