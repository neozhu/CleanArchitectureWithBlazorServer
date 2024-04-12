using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Security;
public class ChangeUserProfileModel
{
    public string? Provider { get; set; }
    public string? SuperiorName { get; set; }
    public string? SuperiorId { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? DisplayName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DefaultRole { get; set; }
    public string[]? AssignedRoles { get; set; }
    public required string UserId { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChangeUserProfileModel, UserProfile>().ReverseMap();
        }
    }
}


public class ChangeUserProfileModelValidator : AbstractValidator<ChangeUserProfileModel>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ChangeUserProfileModelValidator(IStringLocalizer<ApplicationUserDtoValidator> localizer)
    {
        _localizer = localizer;
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["User name cannot be empty"])
            .Length(2, 100).WithMessage(_localizer["User name must be between 2 and 100 characters"]);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail cannot be empty"])
            .MaximumLength(100).WithMessage(_localizer["E-mail must be less than 100 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(128).WithMessage(_localizer["Display name must be less than 128 characters"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage(_localizer["Phone number must be less than 20 digits"]);
    }
}
