// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using CleanArchitecture.Blazor.Domain;
global using CleanArchitecture.Blazor.Domain.Common;
global using CleanArchitecture.Blazor.Domain.Entities;
global using CleanArchitecture.Blazor.Domain.Entities.Audit;
global using CleanArchitecture.Blazor.Domain.Entities.Log;
global using CleanArchitecture.Blazor.$safeprojectname$.Persistence.Extensions;
global using CleanArchitecture.Blazor.Application.Common.Exceptions;
global using CleanArchitecture.Blazor.Application.Common.Models;
global using CleanArchitecture.Blazor.Application.Common.Interfaces;
global using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
global using CleanArchitecture.Blazor.Application.Settings;
global using CleanArchitecture.Blazor.$safeprojectname$.Configurations;
global using CleanArchitecture.Blazor.$safeprojectname$.Constants.ClaimTypes;
global using CleanArchitecture.Blazor.$safeprojectname$.Constants.Localization;
global using CleanArchitecture.Blazor.Application.Constants.Permission;
global using CleanArchitecture.Blazor.$safeprojectname$.Identity;
global using CleanArchitecture.Blazor.$safeprojectname$.Middlewares;
global using CleanArchitecture.Blazor.$safeprojectname$.Persistence;
global using CleanArchitecture.Blazor.$safeprojectname$.Services;
global using CleanArchitecture.Blazor.$safeprojectname$.Services.Identity;
