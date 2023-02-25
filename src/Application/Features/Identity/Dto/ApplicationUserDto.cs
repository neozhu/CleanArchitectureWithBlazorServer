using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Dto;
public class ApplicationUserDto:IMapFrom<ApplicationUser>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
           .ForMember(x => x.SuperiorName, s => s.MapFrom(y => y.Superior.UserName))
           .ForMember(x=>x.AssignedRoles,s=>s.MapFrom(y=>y.UserRoles.Select(r=>r.Role.Name)));
         
    }
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? SuperiorId { get; set; }
    public string? SuperiorName { get; set; }
    public string[]? AssignedRoles { get; set; }
    public string? DefaultRole  => AssignedRoles?.FirstOrDefault();
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public  DateTimeOffset? LockoutEnd { get; set; }
    public UserProfile ToUserProfile() => new()
    {

        UserId = Id,
        ProfilePictureDataUrl = ProfilePictureDataUrl,
        Email = Email,
        PhoneNumber = PhoneNumber,
        DisplayName = DisplayName,
        Provider = Provider,
        UserName = UserName,
        TenantId = TenantId,
        TenantName = TenantName,
        SuperiorId = SuperiorId,
        SuperiorName = SuperiorName,
        AssignedRoles = AssignedRoles,
        DefaultRole = DefaultRole

    };
    public bool IsInRole(string role)=> AssignedRoles?.Contains(role)??false;
}
