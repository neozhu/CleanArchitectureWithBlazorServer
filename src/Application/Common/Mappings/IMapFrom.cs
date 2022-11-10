// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper.Internal;

namespace CleanArchitecture.Blazor.Application.Common.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {
        profile.Internal().MethodMappingEnabled=false;
        profile.CreateMap(typeof(T), GetType(), MemberList.None);
        profile.CreateMap(GetType(), typeof(T), MemberList.None);
    }
}
