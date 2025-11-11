# Serilog 配置迁移说明

## 概述
本次迁移将 Serilog 的配置从 `appsettings.json` 迁移到代码中的 `SerilogExtensions.cs`，提高了配置的可维护性和可读性。

## 迁移内容

### 1. 最小日志级别配置 (MinimumLevel)
以下配置已从 `appsettings.json` 迁移到 `SerilogExtensions.cs`：

```csharp
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
.MinimumLevel.Override("System", LogEventLevel.Warning)
.MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
```

### 2. 日志属性 (Properties)
以下属性已添加到日志配置中：

```csharp
.Enrich.WithProperty("Application", "BlazorApp")
.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
.Enrich.WithProperty("TargetFramework", "net9")
```

注意：`Environment` 属性现在会动态使用当前托管环境名称（Development、Staging、Production 等）。

### 3. Seq 日志接收器配置 (WriteTo Seq)
Seq 配置逻辑已迁移到 `WriteToSeq` 方法中：

```csharp
private static void WriteToSeq(LoggerConfiguration serilogConfig, IConfiguration configuration)
{
    var serverUrl = configuration.GetValue<string>("SerilogSeq:ServerUrl") ?? "https://seq.blazorserver.com";
    var apiKey = configuration.GetValue<string>("SerilogSeq:ApiKey") ?? "none";
    var restrictedToMinimumLevel = configuration.GetValue<string>("SerilogSeq:MinimumLevel") ?? "Verbose";

    if (!string.IsNullOrEmpty(serverUrl))
    {
        var minimumLevel = Enum.TryParse<LogEventLevel>(restrictedToMinimumLevel, true, out var level) 
            ? level 
            : LogEventLevel.Verbose;

        serilogConfig.WriteTo.Seq(
            serverUrl,
            apiKey: string.IsNullOrEmpty(apiKey) || apiKey == "none" ? null : apiKey,
            restrictedToMinimumLevel: minimumLevel
        );
    }
}
```

## 配置文件变更

### appsettings.json
删除了原有的 `Serilog` 配置节，新增了 `SerilogSeq` 配置节：

```json
{
  "SerilogSeq": {
    "ServerUrl": "https://seq.blazorserver.com",
    "ApiKey": "none",
    "MinimumLevel": "Verbose"
  }
}
```

## 优势

1. **代码集中管理**：所有 Serilog 配置逻辑现在都在 `SerilogExtensions.cs` 中，更容易维护和理解。

2. **灵活的配置覆盖**：通过 `SerilogSeq` 配置节，仍然可以在不同环境中覆盖 Seq 配置，无需重新编译。

3. **默认值支持**：代码中提供了合理的默认值，即使配置文件中缺少某些配置也能正常运行。

4. **类型安全**：配置在代码中，可以利用编译器进行类型检查，减少配置错误。

5. **动态环境感知**：`Environment` 属性会自动使用当前托管环境名称。

## 迁移后的配置结构

### 必需配置（在代码中）
- 最小日志级别
- 日志属性（Application, Environment, TargetFramework）
- 文件和控制台输出配置

### 可选配置（在 appsettings.json 中）
- Seq 服务器 URL
- Seq API 密钥
- Seq 最小日志级别

## 使用建议

如果您需要在不同环境中使用不同的 Seq 配置，可以在对应的 `appsettings.{Environment}.json` 文件中覆盖 `SerilogSeq` 配置：

```json
// appsettings.Development.json
{
  "SerilogSeq": {
    "ServerUrl": "http://localhost:5341",
    "ApiKey": "your-dev-api-key",
    "MinimumLevel": "Debug"
  }
}

// appsettings.Production.json
{
  "SerilogSeq": {
    "ServerUrl": "https://seq.blazorserver.com",
    "ApiKey": "your-prod-api-key",
    "MinimumLevel": "Information"
  }
}
```

如果不需要 Seq 日志记录，可以删除 `SerilogSeq` 配置节，`WriteToSeq` 方法会使用默认值。
