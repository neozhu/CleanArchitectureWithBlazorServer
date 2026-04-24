using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum SecurityThreatType
{
    [Display(Name = "New IP Address")]
    NewIpAddress = 1,
    
    [Display(Name = "Suspicious Geographic Location")]
    SuspiciousLocation = 2,
    
    [Display(Name = "Multiple Failed Logins")]
    MultipleFailedLogins = 3,
    
    [Display(Name = "Unusual Login Time")]
    UnusualLoginTime = 4,
    
    [Display(Name = "New Device/Browser")]
    NewDevice = 5,
    
    [Display(Name = "Concurrent Sessions")]
    ConcurrentSessions = 6,
    
    [Display(Name = "Rapid Geographic Movement")]
    RapidGeographicMovement = 7,
    
    [Display(Name = "Account Lockout Events")]
    AccountLockoutEvents = 8
} 
