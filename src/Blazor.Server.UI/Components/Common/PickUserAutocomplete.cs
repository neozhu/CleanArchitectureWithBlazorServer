using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using IdentityModel.Client;

namespace Blazor.Server.UI.Components.Common;

public class PickUserAutocomplete : MudAutocomplete<string>
{
    [Parameter]
    public string TenantId { get; set; }=string.Empty;
    [Inject]
    private IIdentityService  _identityService { get; set; } = default!;

    private List<ApplicationUserDto>? _userList;
  

    public override  Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = searchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 50;
        return base.SetParametersAsync(parameters);
      
    }
    private async Task<IEnumerable<string>> searchKeyValues(string value, CancellationToken cancellation)
    {
        // if text is null or empty, show complete list
        _userList = await _identityService.GetUsers(TenantId, cancellation);
        var result= new List<string>();
        if (_userList is not null && string.IsNullOrEmpty(value))
        {
            result= _userList.Select(x => x.UserName).ToList();
        }
        else if(_userList is not null)
        {
            result = _userList.Where(x => x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(x => x.UserName).ToList();
        }
        return result;
    }
     


}



