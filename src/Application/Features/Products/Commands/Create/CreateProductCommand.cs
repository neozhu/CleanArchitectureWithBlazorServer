// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Products.Commands.Create;

    public class CreateProductCommand: ProductDto,IRequest<Result<int>>, IMapFrom<Product>
    {
       
    }
    
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateProductCommand> _localizer;
        public CreateProductCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateProductCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
           //TODO:Implementing CreateProductCommandHandler method 
           var item = _mapper.Map<Product>(request);
           _context.Products.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  Result<int>.Success(item.Id);
        }
    }

