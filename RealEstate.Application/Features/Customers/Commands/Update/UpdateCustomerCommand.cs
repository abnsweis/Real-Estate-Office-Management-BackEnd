using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Services;
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
    public record UpdateCustomerCommand : IRequest<AppResponse>, ICustomerDTO
    {
        public Guid CustomerId { get; set; }
        public string fullName { get; set; }
        public string nationalId { get; set; }
        public string phoneNumber { get; set; }
        public string dateOfBirth { get; set; }
        public enGender gender { get; set; }
        public enCustomerType customerType { get; set; }
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
            var Customer = await _customerRepository.FirstOrDefaultAsync(filter: u => u.Id == request.CustomerId, includes: x => x.Person);
            if (Customer is null)
            {
                return new AppResponse
                {
                    Result = Result.Fail(new NotFoundError("customer", "customerId", request.CustomerId.ToString(), enApiErrorCode.CustomerNotFound)),
                    Data = request.CustomerId.ToString()
                };
            }


            if (_customerRepository.IsCustomerExists(request.nationalId) && request.nationalId != Customer.Person.NationalId)
            {
                var error = new ConflictError(nameof(request.nationalId), $"A customer with the same national ID is already registered as a {request.customerType}.", enApiErrorCode.DuplicateCustomer);
                errors.Add(error);
            }

            if (_customerRepository.IsCustomerPhoneNumberAlreadyTaken(request.phoneNumber) && request.phoneNumber != Customer.PhoneNumber)
            {
                errors.Add(new ConflictError(nameof(request.phoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }



            if (errors.Any())
            {
                return new AppResponse { Result = Result.Fail(errors) };
            }


            Customer.Person.FullName = request.fullName;
            Customer.Person.NationalId = request.nationalId;
            Customer.Person.Gender = request.gender;
            Customer.Person.DateOfBirth = DateOnly.Parse(request.dateOfBirth);
            Customer.CustomerType = request.customerType;
            Customer.PhoneNumber = request.phoneNumber;


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
