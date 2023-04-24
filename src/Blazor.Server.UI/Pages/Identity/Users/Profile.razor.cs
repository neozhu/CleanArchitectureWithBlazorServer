using Blazor.Server.UI.Services.JsInterop;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace Blazor.Server.UI.Pages.Identity.Users
{
    public partial class Profile
    {

        [Inject]
        private IMediator Mediator { get; set; } = default!;
        private MudForm? _form = null !;
        private MudForm? _Passwordform = null !;
        public string Title { get; set; } = "Profile";
        private bool _submitting;
        [CascadingParameter]
        private Task<AuthenticationState> AuthState { get; set; } = default !;
        [Inject]
        private IUploadService UploadService { get; set; } = default !;
        private UserProfile? Model { get; set; } = null!;
        private readonly UserProfileEditValidator _userValidator = new();
        private UserManager<ApplicationUser> UserManager { get; set; } = default !;
        [Inject]
        private IIdentityService IdentityService { get; set; } = default !;
        public string Id => Guid.NewGuid().ToString();
        private ChangePasswordModel Changepassword { get; set; } = new();
        private readonly ChangePasswordModelValidator _passwordValidator = new();
        private readonly List<OrgItem> _orgData = new();

    
        private async void ActivePanelIndexChanged(int index)
        {
            if (index == 2)
            {
                await LoadOrgData();
                await new OrgChart(JS).Create(_orgData);
            }
        }

        private async Task LoadOrgData()
        {
            var currentUsername = (await AuthState).User.GetUserName();
            var list = await UserManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Superior).ToListAsync();
            foreach (var item in list)
            {
                var roles = await UserManager.GetRolesAsync(item);
                var count = await UserManager.Users.Where(x => x.SuperiorId == item.Id).CountAsync();
                var orgItem = new OrgItem();
                orgItem.Id = item.Id;
                orgItem.Name = item.DisplayName ?? item.UserName;
                orgItem.Area = item.TenantName;
                orgItem.ProfileUrl = item.ProfilePictureDataUrl;
                orgItem.ImageUrl = item.ProfilePictureDataUrl;
                if (currentUsername == item.UserName)
                    orgItem.IsLoggedUser = true;
                orgItem.Size = "";
                orgItem.Tags = item.PhoneNumber ?? item.Email;
                if (roles != null && roles.Count > 0)
                    orgItem.PositionName = string.Join(',', roles);
                orgItem.ParentId = item.SuperiorId;
                orgItem.DirectSubordinates = count;
                this._orgData.Add(orgItem);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            UserManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
            var userId = (await AuthState).User.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var userDto = await IdentityService.GetApplicationUserDto(userId);
                this.Model = userDto.ToUserProfile();
            }
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
                    image.Mutate(i => i.Resize(new ResizeOptions() { Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop, Size = new SixLabors.ImageSharp.Size(128, 128) }));
                    image.Save(outStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
                    var filename = e.File.Name;
                    var fi = new FileInfo(filename);
                    var ext = fi.Extension;
                    var result = await UploadService.UploadAsync(new UploadRequest(Guid.NewGuid().ToString() + ext, UploadType.ProfilePicture, outStream.ToArray()));
                    Model.ProfilePictureDataUrl = result;
                    var user = await UserManager.FindByIdAsync(Model.UserId!);
                    user!.ProfilePictureDataUrl = Model.ProfilePictureDataUrl;
                    await UserManager.UpdateAsync(user);
                    Snackbar.Add(L["The avatar has been updated."], MudBlazor.Severity.Info);
                   await  Mediator.Publish(new UpdateUserProfileCommand() { UserProfile = Model });
                }
            }
        }

        private async Task Submit()
        {
            _submitting = true;
            try
            {
                await _form!.Validate();
                if (_form.IsValid)
                {
                    var state = await AuthState;
                    var user = await UserManager.FindByIdAsync(Model.UserId!);
                    user!.PhoneNumber = Model.PhoneNumber;
                    user.DisplayName = Model.DisplayName;
                    user.ProfilePictureDataUrl = Model.ProfilePictureDataUrl;
                    await UserManager.UpdateAsync(user);
                    Snackbar.Add($"{ConstantString.UpdateSuccess}", MudBlazor.Severity.Info);
                    await Mediator.Publish(new UpdateUserProfileCommand() { UserProfile = Model });
                }
            }
            finally
            {
                _submitting = false;
            }
        }

        private async Task ChangePassword()
        {
            _submitting = true;
            try
            {
                await _Passwordform!.Validate();
                if (_Passwordform!.IsValid)
                {
                    var user = await UserManager.FindByIdAsync(Model.UserId!);
                    var result = await UserManager.ChangePasswordAsync(user!, Changepassword.CurrentPassword, Changepassword.NewPassword);
                    if (result.Succeeded)
                    {
                        Snackbar.Add($"{L["Changed password successfully."]}", MudBlazor.Severity.Info);
                    }
                    else
                    {
                        Snackbar.Add($"{string.Join(",", result.Errors.Select(x => x.Description).ToArray())}", MudBlazor.Severity.Error);
                    }
                }
            }
            finally
            {
                _submitting = false;
            }
        }
        
    }
}