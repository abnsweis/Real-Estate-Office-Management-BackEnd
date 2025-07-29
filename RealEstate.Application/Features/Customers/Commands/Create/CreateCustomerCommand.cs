using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Customers.Commands.Create
{
    public class CreateCustomerCommand : IRequest<AppResponse<Guid>>, ICustomerDTO
    {
        public string fullName { get; set; }
        public string nationalId { get; set; }
        public string phoneNumber { get; set; }
        public string dateOfBirth { get; set; }
        public enGender gender { get; set; }
        public enCustomerType customerType { get; set; }
    }


    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, AppResponse<Guid>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        private string _Imagepath = string.Empty;
        public CreateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IPersonRepository personRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }
        public async Task<AppResponse<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new List<Error>();



            var existingCustomer = _customerRepository.GetCustomerByNationalId(request.nationalId);

            if (existingCustomer == null && _customerRepository.IsCustomerPhoneNumberAlreadyTaken(request.phoneNumber))
            {
                errors.Add(new ConflictError(nameof(request.phoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }

            if (_customerRepository.IsCustomerExists(request.nationalId, request.customerType))
            {
                var error = new ConflictError(nameof(request.nationalId), $"A customer with the same national ID is already registered as a {request.customerType}.", enApiErrorCode.DuplicateCustomer);
                return new AppResponse<Guid> { Result = Result.Fail(error) };
            }

            Customer Customer;
            if (existingCustomer != null)
            {
                Customer = new Customer
                {

                    PersonId = existingCustomer.PersonId,
                    PhoneNumber = existingCustomer.PhoneNumber,
                    CustomerType = request.customerType,

                };

            } else
            {
                var result = _fileManager.SetDefaultUserProfileImage();
                if (result.IsSuccess)
                {
                    _Imagepath = result.Value;
                } else
                {
                    errors.AddRange(result.Errors.Cast<Error>());
                    return new AppResponse<Guid> { Result = Result.Fail(errors), Data = Guid.Empty };
                }
                Customer = new Customer()
                {

                    PhoneNumber = request.phoneNumber,
                    CustomerType = request.customerType,
                    Person = new Person
                    {
                        FullName = request.fullName,
                        NationalId = request.nationalId,
                        DateOfBirth = DateOnly.Parse(request.dateOfBirth),
                        Gender = request.gender,
                        ImageURL = _Imagepath,
                    }

                };
            }


            if (errors.Any())
            {
                _fileManager.DeleteFile(_Imagepath);
                return new AppResponse<Guid> { Result = Result.Fail(errors) };
            }
            var NewCustomer = await _customerRepository.AddAsync(Customer);
            await _customerRepository.SaveChangesAsync();


            return new AppResponse<Guid> { Result = Result.Ok(), Data = NewCustomer.Id };
        }
    }

}
