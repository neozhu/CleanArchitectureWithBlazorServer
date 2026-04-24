using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Common.Enums;

public enum JobStatus
{

    [Display(Name = "Not Started")]
    NotStart,

    [Display(Name = "Queuing")]
    Queueing,


    [Display(Name = "In Progress")]
    Doing,


    [Display(Name = "Completed")]
    Done,

    [Display(Name = "Pending")]
    Pending
}
