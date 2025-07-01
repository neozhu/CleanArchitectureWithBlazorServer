# IP 地理位置服务使用说明

本项目中已实现了一个完整的IP地理位置服务，用于调用 `http://ip-api.com/json/` API获取详细地理位置信息。该服务符合Clean Architecture框架设计。

## 服务结构

### 1. 接口定义 (Application层)
- **文件位置**: `src/Application/Common/Interfaces/IGeolocationService.cs`
- **接口**: `IGeolocationService`
- **功能**: 
  - `GetCountryAsync(string ipAddress)` - 获取国家代码 (使用 ipapi.co)
  - `GetGeolocationAsync(string ipAddress)` - 获取详细地理位置信息 (使用 ip-api.com)

### 2. 服务实现 (Infrastructure层)
- **文件位置**: `src/Infrastructure/Services/GeolocationService.cs`
- **类**: `GeolocationService`
- **特性**:
  - 使用HttpClient调用ip-api.com API获取详细信息
  - 使用HttpClient调用ipapi.co API获取国家代码
  - 包含完整的错误处理和日志记录
  - 支持超时和取消令牌
  - 返回结构化的地理位置信息

### 3. 扩展方法
- **文件位置**: `src/Infrastructure/Extensions/LoginAuditExtensions.cs`
- **功能**: 为LoginAudit实体提供地理位置增强方法

### 4. 辅助服务
- **文件位置**: `src/Infrastructure/Services/LoginAuditService.cs`
- **功能**: 演示如何在实际业务场景中使用地理位置服务

## 使用示例

### 1. 基本用法 - 获取国家代码

```csharp
public class ExampleController : ControllerBase
{
    private readonly IGeolocationService _geolocationService;

    public ExampleController(IGeolocationService geolocationService)
    {
        _geolocationService = geolocationService;
    }

    [HttpGet("country/{ip}")]
    public async Task<string?> GetCountry(string ip)
    {
        return await _geolocationService.GetCountryAsync(ip);
    }
}
```

### 2. 获取详细地理位置信息

```csharp
[HttpGet("location/{ip}")]
public async Task<GeolocationInfo?> GetLocation(string ip)
{
    return await _geolocationService.GetGeolocationAsync(ip);
}
```

### 3. 在登录审计中使用

```csharp
public class AuthenticationService
{
    private readonly IGeolocationService _geolocationService;

    public async Task<LoginAudit> CreateLoginAudit(string userId, string userName, string ipAddress)
    {
        var loginAudit = new LoginAudit
        {
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            LoginTimeUtc = DateTimeOffset.UtcNow
        };

        // 使用扩展方法添加地理位置信息
        await loginAudit.EnrichWithGeolocationAsync(_geolocationService);
        
        return loginAudit;
    }
}
```

### 4. 使用LoginAuditService

```csharp
public class LoginController : ControllerBase
{
    private readonly LoginAuditService _loginAuditService;

    public LoginController(LoginAuditService loginAuditService)
    {
        _loginAuditService = loginAuditService;
    }

    [HttpPost("audit")]
    public async Task<LoginAudit> CreateAudit([FromBody] LoginRequest request)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        return await _loginAuditService.CreateLoginAuditAsync(
            request.UserId,
            request.UserName,
            clientIp,
            Request.Headers["User-Agent"].ToString(),
            "Internal",
            true
        );
    }
}
```

## API响应示例

### GetCountryAsync 响应
```
CN
```

### GetGeolocationAsync 响应
使用 ip-api.com API返回的详细信息：
```json
{
  "ipAddress": "24.48.0.1",
  "country": "CA",
  "countryName": "Canada",
  "region": "Quebec",
  "city": "Montreal",
  "postalCode": "H1L",
  "latitude": 45.6026,
  "longitude": -73.5167,
  "timezone": "America/Toronto",
  "isp": "Le Groupe Videotron Ltee",
  "organization": "Videotron Ltee"
}
```

## 配置说明

服务已在 `Infrastructure/DependencyInjection.cs` 中注册：

```csharp
services.AddHttpClient<IGeolocationService, GeolocationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "CleanArchitectureBlazorServer/1.0");
});
```

## 错误处理

服务包含完整的错误处理：
- HTTP请求异常
- 超时处理
- JSON反序列化错误
- 无效IP地址处理
- API错误响应处理

## 日志记录

服务使用Microsoft.Extensions.Logging进行日志记录：
- Debug级别：成功请求的详细信息
- Warning级别：API错误或无效响应
- Error级别：异常情况

## 注意事项

1. **API选择**: 
   - `GetCountryAsync` 使用 ipapi.co API (有免费使用限制，每天1000次请求)
   - `GetGeolocationAsync` 使用 ip-api.com API (免费版本每分钟45次请求，无商业用途限制)
2. **网络依赖**: 服务依赖外部API，需要网络连接
3. **性能考虑**: 建议在非关键路径中使用，或实现缓存机制
4. **隐私考虑**: 确保遵守相关隐私法规处理IP地址信息
5. **API稳定性**: ip-api.com 提供更稳定的服务和更详细的地理位置信息
