using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public abstract class BaseAuthStore
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public delegate void OnSaveEventHandler(object sender, BaseAuthStore authStore);
    public event OnSaveEventHandler? OnSave;

    public void Save(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        OnSave?.Invoke(this, this);
    }

    public void Clear()
    {
        AccessToken = null;
        AccessToken = null;
        OnSave?.Invoke(this, this);
    }

    

     
}