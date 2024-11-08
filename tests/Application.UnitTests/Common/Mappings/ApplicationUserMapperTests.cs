using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Blazor.Application.Features.Identity.Mappers;
using CleanArchitecture.Blazor.Domain.Identity;
using NUnit.Framework.Legacy;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Mappings;
[TestFixture]
public class ApplicationUserMapperTests
{
    [Test]
    public void ToApplicationUserDto_Should_Map_Properties_Correctly()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "testuser@example.com",
            UserRoles = new List<ApplicationUserRole>
                {
                    new ApplicationUserRole
                    {
                        Role = new ApplicationRole { Name = "Admin" }
                    },
                    new ApplicationUserRole
                    {
                        Role = new ApplicationRole { Name = "User" }
                    }
                },
            // Initialize other properties as needed
        };

        // Act
        var userDto = ApplicationUserMapper.ToApplicationUserDto(user);

        // Assert
        Assert.That(userDto!=null);
        Assert.That(user.Id, Is.EqualTo(userDto.Id));
        Assert.That(user.UserName, Is.EqualTo(userDto.UserName));
        Assert.That(user.Email, Is.EqualTo(userDto.Email));
        Assert.That(userDto.AssignedRoles.Length, Is.EqualTo(2));
        CollectionAssert.AreEquivalent(new[] { "Admin", "User" }, userDto.AssignedRoles);
        // Add more assertions for other properties if necessary
    }

    [Test]
    public void ProjectTo_Should_Project_IQueryable_Correctly()
    {
        // Arrange
        var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "testuser1",
                    Email = "testuser1@example.com",
                    UserRoles = new List<ApplicationUserRole>
                    {
                        new ApplicationUserRole
                        {
                            Role = new ApplicationRole { Name = "Admin" }
                        }
                    },
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "testuser2",
                    Email = "testuser2@example.com",
                    UserRoles = new List<ApplicationUserRole>
                    {
                        new ApplicationUserRole
                        {
                            Role = new ApplicationRole { Name = "User" }
                        }
                    },
                }
            }.AsQueryable();

        // Act
        var userDtos = users.ProjectTo().ToList();

        // Assert
        Assert.That(userDtos.Any());
        Assert.That(users.Count()==userDtos.Count);

        for (int i = 0; i < users.Count(); i++)
        {
            var user = users.ElementAt(i);
            var userDto = userDtos[i];

            Assert.That(user.Id, Is.EqualTo(userDto.Id));
            Assert.That(user.UserName, Is.EqualTo(userDto.UserName));
            Assert.That(user.Email, Is.EqualTo(userDto.Email));
            Assert.That(userDto.AssignedRoles.Length, Is.EqualTo(1));
            Assert.That(user.UserRoles.First().Role.Name, Is.EqualTo(userDto.AssignedRoles.First()));
        }
    }
}