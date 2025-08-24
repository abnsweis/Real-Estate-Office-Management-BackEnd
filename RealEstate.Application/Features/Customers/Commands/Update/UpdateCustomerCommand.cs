using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Services;
using RealEstate.Application.Dtos.Customer;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = FluentResults.Error;

namespace RealEstate.Application.Features.Customers.Commands.Update
{
    public record UpdateCustomerCommand : IRequest<AppResponse> 
    {
        public UpdateCustomerCommand(UpdateCustomerDTO Data)
        {
            this.Data = Data;
        }

        public UpdateCustomerDTO Data { get; }
    }


    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, AppResponse>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public UpdateCustomerCommandHandler(
           ICustomerRepository customerRepository,
           IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<AppResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new List<Error>();
            var Customer = await _customerRepository.FirstOrDefaultAsync(filter: u => u.Id == request.Data.GetCustomerId(), includes: x => x.Person);
            if (Customer is null)
            {
                return new AppResponse
                {
                    Result = Result.Fail(new NotFoundError("customer", "customerId", request.Data.GetCustomerId().ToString(), enApiErrorCode.CustomerNotFound)),
                    Data = request.Data.GetCustomerId().ToString()
                };
            }


            if (_customerRepository.IsCustomerExists(request.Data.nationalId) && request.Data.nationalId != Customer.Person.NationalId)
            {
                var error = new ConflictError(nameof(request.Data.nationalId), $"A customer with the same national ID is already registered as a {request.Data.customerType}.", enApiErrorCode.DuplicateCustomer);
                errors.Add(error);
            }

            if (_customerRepository.IsCustomerPhoneNumberAlreadyTaken(request.Data.phoneNumber) && request.Data.phoneNumber != Customer.PhoneNumber)
            {
                errors.Add(new ConflictError(nameof(request.Data.phoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }



            if (errors.Any())
            {
                return new AppResponse { Result = Result.Fail(errors) };
            }


            Customer.Person.FullName = request.Data.fullName!;
            Customer.Person.NationalId = request.Data.nationalId;
            Customer.Person.Gender = request.Data.gender.Value;
            Customer.Person.DateOfBirth = DateOnly.Parse(request.Data.dateOfBirth);
            Customer.CustomerType = request.Data.customerType.Value;
            Customer.PhoneNumber = request.Data.phoneNumber;


            await _customerRepository.UpdateAsync(Customer);

            await _customerRepository.SaveChangesAsync();

            return new AppResponse
            {
                Result = Result.Ok(),
                Data = _mapper.Map<CustomerDTO>(Customer)
            };
        }
    }
}
