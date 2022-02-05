// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using CleanArchitecture.Razor.Domain.Common;

namespace CleanArchitecture.Razor.Application.Common.Interfaces;

public interface ISpecification<T> where T : class, IEntity
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, bool>> And(Expression<Func<T, bool>> query);
    Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query);
}
