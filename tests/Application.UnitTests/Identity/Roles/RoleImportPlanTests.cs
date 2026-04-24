#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CleanArchitecture.Blazor.Domain.Identity;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Identity.Roles;

public class RoleImportPlanTests
{
    [Test]
    public void GetRolesToCreate_ShouldSkipExistingAndDuplicateNormalizedNames()
    {
        var importedRoles = new[]
        {
            new ApplicationRole { Name = "Admin", Description = "existing" },
            new ApplicationRole { Name = "User", Description = "new" },
            new ApplicationRole { Name = " user ", Description = "duplicate in file" }
        };
        var existingNormalizedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ADMIN" };

        var result = InvokeGetRolesToCreate(importedRoles, existingNormalizedNames);

        result.SkippedCount.Should().Be(2);
        result.InvalidCount.Should().Be(0);
        result.RoleNames.Should().Equal("User");
    }

    [Test]
    public void GetRolesToCreate_ShouldTreatBlankNamesAsInvalid()
    {
        var importedRoles = new[]
        {
            new ApplicationRole { Name = "  " },
            new ApplicationRole { Name = "Guest" }
        };

        var result = InvokeGetRolesToCreate(importedRoles, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        result.SkippedCount.Should().Be(0);
        result.InvalidCount.Should().Be(1);
        result.RoleNames.Should().Equal("Guest");
    }

    private static (IReadOnlyList<string?> RoleNames, int SkippedCount, int InvalidCount) InvokeGetRolesToCreate(
        IEnumerable<ApplicationRole> importedRoles,
        ISet<string> existingNormalizedNames)
    {
        var rolesType = Type.GetType("CleanArchitecture.Blazor.Server.UI.Pages.Identity.Roles.Roles, CleanArchitecture.Blazor.Server.UI");
        rolesType.Should().NotBeNull();

        var method = rolesType!.GetMethod("GetRolesToCreate", BindingFlags.Static | BindingFlags.NonPublic);
        method.Should().NotBeNull();

        var args = new object?[]
        {
            importedRoles,
            existingNormalizedNames,
            new Func<string, string?>(value => value.Trim().ToUpperInvariant()),
            0,
            0
        };

        var roles = method!.Invoke(null, args);
        roles.Should().BeAssignableTo<IEnumerable>();

        return (
            ((IEnumerable)roles!).Cast<ApplicationRole>().Select(role => role.Name).ToList(),
            (int)args[3]!,
            (int)args[4]!
        );
    }
}
