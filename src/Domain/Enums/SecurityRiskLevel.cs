using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum SecurityRiskLevel
{
    [Display(Name ="Low")]
    Low = 1,
    
    [Display(Name ="Medium")]
    Medium = 2,
    
    [Display(Name = "High")]
    High = 3,
    
    [Display(Name = "Critical")]
    Critical = 4
} 
