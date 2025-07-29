using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Customers.Commands.Update
{
    public record UpdatePropertyStatusToCommand : IRequest<AppResponse> 
    {
        public string Status { get; }
        public Guid PropertyId { get; }

        public UpdatePropertyStatusToCommand(string Status,Guid propertyId)
        {
            this.Status = Status;
            this.PropertyId = propertyId;
        }
    }

    public class UpdatePropertyStatusToCommandHandler : IRequestHandler<UpdatePropertyStatusToCommand, AppResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        public UpdatePropertyStatusToCommandHandler(
           IPropertyRepository propertyRepository,
           IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<AppResponse> Handle(UpdatePropertyStatusToCommand request, CancellationToken cancellationToken)
        {

            var property = await _propertyRepository.FirstOrDefaultAsync(filter: u => u.Id == request.PropertyId);
            if (property is null)
            {
                return new AppResponse
                {
                    Result = Result.Fail(new NotFoundError("customer", "customerId", request.PropertyId.ToString(), enApiErrorCode.CustomerNotFound)),
                    Data = request.PropertyId.ToString()
                };
            }

 

            if (!Enum.TryParse<enPropertyStatus>(request.Status,out var enStatus))
            {
                var error = new ValidationError("PropertiesStatus", $"Invlaid Properties Status Value {request.Status}", enApiErrorCode.InvalidEnumValue);
                return AppResponse.Fail(error);

            }

            property.PropertyStatus = enStatus;

             await _propertyRepository.SaveChangesAsync();
            return AppResponse.Success();
             
        }
    }

    
}
