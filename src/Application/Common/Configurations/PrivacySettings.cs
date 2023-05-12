namespace CleanArchitecture.Blazor.Application.Common.Configurations;

public class PrivacySettings
{
    public const string Privacy = "Privacy";
    
    public bool LogClientIpAddresses { get; set; } = true;
    public bool LogClientAgents { get; set; } = true;
}