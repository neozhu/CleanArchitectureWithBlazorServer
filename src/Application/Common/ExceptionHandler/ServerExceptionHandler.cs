using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandler;
public class ServerExceptionHandler : IRequestExceptionHandler<AddEditProductCommand, Result<int>, ServerException>
{
    public Task Handle(AddEditProductCommand request, ServerException exception, RequestExceptionHandlerState<Result<int>> state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
