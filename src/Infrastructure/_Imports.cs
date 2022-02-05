// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Server;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Logging;
global using CleanArchitecture.Blazor.Application.Common.Exceptions;
global using CleanArchitecture.Blazor.Application.Common.Models;
global using CleanArchitecture.Blazor.Application.Common.Interfaces;
global using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
global using CleanArchitecture.Blazor.Application.Settings;
global using CleanArchitecture.Blazor.Infrastructure.Configurations;
global using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
global using CleanArchitecture.Blazor.Infrastructure.Constants.Localization;
global using CleanArchitecture.Blazor.Application.Constants.Permission;
global using CleanArchitecture.Blazor.Infrastructure.Identity;
global using CleanArchitecture.Blazor.Infrastructure.Middlewares;
global using CleanArchitecture.Blazor.Infrastructure.Persistence;
global using CleanArchitecture.Blazor.Infrastructure.Services;
global using CleanArchitecture.Blazor.Infrastructure.Services.Identity;
