using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum SecurityThreatType
{
    [Description("New IP Address")]
    NewIpAddress = 1,
    
    [Description("Suspicious Geographic Location")]
    SuspiciousLocation = 2,
    
    [Description("Multiple Failed Logins")]
    MultipleFailedLogins = 3,
    
    [Description("Unusual Login Time")]
    UnusualLoginTime = 4,
    
    [Description("New Device/Browser")]
    NewDevice = 5,
    
    [Description("Concurrent Sessions")]
    ConcurrentSessions = 6,
    
    [Description("Rapid Geographic Movement")]
    RapidGeographicMovement = 7,
    
    [Description("Account Lockout Events")]
    AccountLockoutEvents = 8
} 
