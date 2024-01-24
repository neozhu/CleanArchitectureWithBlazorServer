namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public abstract class BaseAuthStore
{
    public delegate void OnSaveEventHandler(object sender, BaseAuthStore authStore);

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
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