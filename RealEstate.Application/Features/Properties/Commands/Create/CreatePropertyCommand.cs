using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;  

namespace RealEstate.Application.Features.Properties.Commands.Create
{
    public class CreatePropertyCommand : IRequest<AppResponse<Guid>>  
    {
        public CreatePropertyDTO Data { get; }
          
        public CreatePropertyCommand(CreatePropertyDTO data)
        {
            Data = data;
        }

    }



    public class CreatePropertyCommandHandler : Base, IRequestHandler<CreatePropertyCommand, AppResponse<Guid>>
    {
 
        public CreatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
            ICategoryRepository categoryRepository,
            IPropertyImageRepository propertyImageRepository,
            IFileManager fileManager,
            IMapper mapper
            )
            : base(propertyRepository,customerRepository,categoryRepository, propertyImageRepository, fileManager,mapper)
        {
     
        }

        public async Task<AppResponse<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var validationResults = ValidetePropertyDate(request);

            if (validationResults.IsFailed)
            {
                return AppResponse<Guid>.Fail(validationResults.Errors);
            }

            var propertyNumber = await GenerateUniquePropertyNumberAsync();

            var coustomer= _customerRepository.GetCustomerByNationalId(request.Data.OwnerNationalId!);

            Property property = _mapper.Map<Property>(request.Data);

            property.PropertyNumber = propertyNumber;
            property.PropertyStatus = PropertyStatus.Available;
            property.OwnerId = coustomer.Id;


            var createVideoPathResults = await PerformPropertyVideoAsync(request.Data.Video);

            if (createVideoPathResults.IsFailed) return AppResponse<Guid>.Fail(createVideoPathResults.Errors);

            property.VideoUrl = createVideoPathResults.Value;

            try
            {
                await _propertyRepository.AddAsync(property);
                var rowsAffacted = await _propertyRepository.SaveChangesAsync();


                if (rowsAffacted > 0)
                {
                    var ImagesPath = await PerformPropertyImages(request.Data.Images);

                    if (ImagesPath.IsFailed) return AppResponse<Guid>.Fail(ImagesPath.Errors);
                     

                    var SaveingImagesResults = await SavePropertyImages(property.Id,ImagesPath.Value);
                    if (SaveingImagesResults.IsFailed) return AppResponse<Guid>.Fail(SaveingImagesResults.Errors);
                }



            }
            catch(Exception ex)
            {
                var error = new InternalServerError("Server",ex.Message,enApiErrorCode.UnexpectedEntityCreationFailure);
                return AppResponse<Guid>.Fail(error);
            }




            return AppResponse<Guid>.Success(property.Id);
        }





    
    }

}
