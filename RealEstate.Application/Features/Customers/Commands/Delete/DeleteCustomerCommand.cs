using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Commands.Delete
{
    public record DeleteCustomerCommand(Guid CustomerId) : IRequest<AppResponse>;

    class DeleteCommandHandler : IRequestHandler<DeleteCustomerCommand, AppResponse>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCommandHandler(ICustomerRepository userRepository)
        {
            _customerRepository = userRepository;
        }

        public async Task<AppResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {


            var customer = await _customerRepository.FirstOrDefaultAsync(customer => customer.Id == request.CustomerId && customer.IsDeleted == false);
            if (customer is null)
            {

                var result = Result.Fail(new NotFoundError(
                    "customer",
                    "customerId",
                    request.CustomerId.ToString(),
                    enApiErrorCode.CustomerNotFound));

                return new AppResponse { Result = result, Data = request.CustomerId };
            }

            _customerRepository.Delete(customer);

            await _customerRepository.SaveChangesAsync(cancellationToken);
            return new AppResponse
            {
                Result = Result.Ok()
            };
        }

    }

}
