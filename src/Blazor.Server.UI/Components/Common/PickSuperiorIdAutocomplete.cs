using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using IdentityModel.Client;

namespace Blazor.Server.UI.Components.Common;

public class PickSuperiorIdAutocomplete : MudAutocomplete<string>
{
    [Parameter]
    public string TenantId { get; set; }=string.Empty;
    [Parameter]
    public string OwnerName { get; set; }=string.Empty;

    [Inject]
    private IIdentityService  _identityService { get; set; } = default!;

    private List<ApplicationUserDto>? _userList;
  

    public override  Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = searchKeyValues;
        ToStringFunc = toString;
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
        if (string.IsNullOrEmpty(value) && _userList is not null)
        {
            result= _userList.Select(x => x.Id).Take(MaxItems??50).ToList();
        }
        else if(_userList is not null)
        {
            result = _userList.Where(x => !x.UserName.Equals(OwnerName, StringComparison.OrdinalIgnoreCase) && (x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) || x.Email.Contains(value, StringComparison.OrdinalIgnoreCase))).Select(x => x.Id).Take(MaxItems ?? 50).ToList(); ;
        }
        return result;
    }
    private string toString(string str)
    {
        if(_userList is not null && !string.IsNullOrEmpty(str) && _userList.Any(x => x.Id.Equals(str, StringComparison.OrdinalIgnoreCase)))
        {
            var userDto = _userList.First(x => x.Id==str);
            return userDto.UserName;
        }
        if(_userList is null && !string.IsNullOrEmpty(str))
        {
            var userName = _identityService.GetUserName(str);
            return userName;
        }
        
        return string.Empty;
    }


}



