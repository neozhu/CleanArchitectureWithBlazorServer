// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        ICurrentUserAccessor  currentUserAccessor,
        IIdentityService identityService)
    {
        _currentUserAccessor = currentUserAccessor;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<RequestAuthorizeAttribute>();
        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            var userId = _currentUserAccessor.SessionInfo?.UserId;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();

            // DefaultRole-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                foreach (var role in roles)
                {
                    var isInRole = await _identityService.IsInRoleAsync(userId, role.Trim());
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized) throw new ForbiddenException("You are not authorized to access this resource.");
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            if (authorizeAttributesWithPolicies.Any())
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(userId, policy);

                    if (!authorized) throw new ForbiddenException("You are not authorized to access this resource.");
                }
        }

        // User is authorized / authorization not required
        return await next().ConfigureAwait(false);
    }
}