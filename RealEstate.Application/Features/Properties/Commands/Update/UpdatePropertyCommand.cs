using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using Error = FluentResults.Error;
using RealEstate.Application.Features.Properties.Commands.Create;
using RealEstate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Entities;
using RealEstate.Application.Features.Properties.Commands;

namespace RealEstate.Application.Features.Propertys.Commands.Update
{
    public record UpdatePropertyCommand : IRequest<AppResponse> 
    {
        public Guid PropertyId { get; }
        public UpdatePropertyDTO Data { get; }
         

        public UpdatePropertyCommand(UpdatePropertyDTO data, Guid propertyId)
        {
            Data = data;
            PropertyId = propertyId;
        }
    }


    public class UpdatePropertyCommandHandler :Base,IRequestHandler<UpdatePropertyCommand, AppResponse>
    {
        public UpdatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
            ICategoryRepository categoryRepository,
            IPropertyImageRepository propertyImageRepository,
            IFileManager fileManager,
            IMapper mapper
            )
            : base(propertyRepository, customerRepository, categoryRepository, propertyImageRepository, fileManager, mapper)
        {

        }

        public async Task<AppResponse> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var validationResults = ValidetePropertyDate(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }


            var property = await _propertyRepository.FirstOrDefaultAsync(filter : p => p.Id == request.PropertyId,includes : p=>p.PropertyImages);

            property!.Title = request.Data.Title!; 
            property!.CategoryId = Guid.TryParse(request.Data.CategoryId,out var categoryId) ? categoryId : Guid.Empty ; 
            property!.Price = (decimal)request.Data.Price!; 
            property!.Location = request.Data.Location!; 
            property!.Address = request.Data.Address!; 
            property!.Area = (decimal)request.Data.Area!; 
            property!.Description = request.Data.Description!;

            if (property.VideoUrl != null)
            {
                _fileManager.DeleteFile(property.VideoUrl);
            }

            var createVideoPathResults = await PerformPropertyVideoAsync(request.Data.Video);

            if (createVideoPathResults.IsFailed) return AppResponse.Fail(createVideoPathResults.Errors);

            property.VideoUrl = createVideoPathResults.Value;

            try
            {
                await _propertyRepository.UpdateAsync(property);
                var rowsAffacted = await _propertyRepository.SaveChangesAsync();


                if (rowsAffacted > 0)
                {


                    foreach (var img in property.PropertyImages)
                    {
                        _fileManager.DeleteFile(img.ImageUrl);  
                    }

                    var ImagesPath = await PerformPropertyImages(request.Data.Images);

                    if (ImagesPath.IsFailed) return AppResponse.Fail(ImagesPath.Errors);


                    var SaveingImagesResults = await SavePropertyImages(property.Id, ImagesPath.Value);
                    if (SaveingImagesResults.IsFailed) return AppResponse.Fail(SaveingImagesResults.Errors);
                }



            }
            catch (Exception ex)
            {
                var error = new InternalServerError("Server", ex.Message, enApiErrorCode.UnexpectedEntityCreationFailure);
                return AppResponse.Fail(error);
            }




            return AppResponse.Success(property.Id);
        }
 

        Result ValidetePropertyDate(UpdatePropertyCommand request)
        {
            List<Error> errors = new();

            if (!_propertyRepository.IsPropertyExistsById(request.PropertyId))
            {
                return Result.Fail(new ValidationError("PropertyId", $"Not Found Property With Id {request.PropertyId}", enApiErrorCode.PropertyNotFound));
            }
            if (!_categoryRepository.IsCategoryExists(Guid.Parse(request.Data.CategoryId!)))
            {
                errors.Add(new ValidationError("CategoryId", $"Not Found Category With Id {request.Data.CategoryId}", enApiErrorCode.CategoryNotFound));
            }
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

         
    }
}
