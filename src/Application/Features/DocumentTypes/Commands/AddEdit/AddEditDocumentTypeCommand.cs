// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit;

public class AddEditDocumentTypeCommand : DocumentTypeDto, IRequest<Result<int>>, ICacheInvalidator
{
    public CancellationTokenSource? SharedExpiryTokenSource => DocumentTypeCacheKey.SharedExpiryTokenSource;
}

public class AddEditDocumentTypeCommandHandler : IRequestHandler<AddEditDocumentTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddEditDocumentTypeCommandHandler(
        IApplicationDbContext context,
         IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(AddEditDocumentTypeCommand request, CancellationToken cancellationToken)
    {


        if (request.Id > 0)
        {
            var documentType = await _context.DocumentTypes.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = documentType ?? throw new NotFoundException($"Document Type {request.Id} Not Found.");
            documentType = _mapper.Map(request, documentType);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(documentType.Id);
        }
        else
        {
            var documentType = _mapper.Map<DocumentType>(request);
            _context.DocumentTypes.Add(documentType);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(documentType.Id);
        }


    }
}
