// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
 
           RuleFor(v => v.Id).NotNull().GreaterThan(0);
      
        }
    }
    public class DeleteCheckedProductsCommandValidator : AbstractValidator<DeleteCheckedProductsCommand>
    {
        public DeleteCheckedProductsCommandValidator()
        {
      
            RuleFor(v => v.Id).NotNull().NotEmpty();
    
        }
    }

