using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Features.Properties.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Properties.Commands
{
    public class Base
    {
        protected readonly IPropertyRepository _propertyRepository;
        protected readonly ICustomerRepository _customerRepository;
        protected readonly ICategoryRepository _categoryRepository;
        protected readonly IPropertyImageRepository _propertyImageRepository;
        protected readonly IFileManager _fileManager;
        protected readonly IMapper _mapper;

        public Base(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
            ICategoryRepository categoryRepository,
            IPropertyImageRepository propertyImageRepository,
            IFileManager fileManager,
            IMapper mapper
            )
        {
            this._propertyRepository = propertyRepository;
            this._customerRepository = customerRepository;
            this._categoryRepository = categoryRepository;
            this._propertyImageRepository = propertyImageRepository;
            this._fileManager = fileManager;
            this._mapper = mapper;
        }
        protected Result ValidetePropertyDate(CreatePropertyCommand request)
        {
            List<Error> errors = new();

            if (!_customerRepository.IsCustomerExists(Guid.Parse(request.Data.OwnerId!)))
            {
                errors.Add(new ValidationError("OwnerId", $"Not Found Owner With Id {request.Data.OwnerId}", enApiErrorCode.CustomerNotFound));
            }

            if (!_categoryRepository.IsCategoryExists(Guid.Parse(request.Data.CategoryId!)))
            {
                errors.Add(new ValidationError("CategoryId", $"Not Found Category With Id {request.Data.CategoryId}", enApiErrorCode.CategoryNotFound));
            }

            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }


        protected async Task<string> GenerateUniquePropertyNumberAsync()
        {
            long number;
            Random random = new Random();

            do
            {
                number = (long)(random.Next(10000000, 99999999)) * 10 + random.Next(0, 10);
            } while (await _propertyRepository.IsPropertyyExistsByPropertyNumber(number.ToString()));

            return number.ToString();
        }

        protected async Task<Result<string>> PerformPropertyVideoAsync(IFormFile video)
        {
            if (video != null)
            {
                var result = await _fileManager.SavePropertyVideoAsync(video);

                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                return Result.Ok(result.Value);
            }

            return Result.Ok<string>(null);
        }

        protected async Task<Result<List<string>>> PerformPropertyImages(List<IFormFile> images)
        {
            List<string> ImagesPath = new List<string>();

            foreach (var img in images)
            {
                var results = await _fileManager.SavePropertyImageAsync(img);

                if (results.IsFailed)
                {
                    return Result.Fail(results.Errors);
                }

                ImagesPath.Add(results.Value);
            }
            return Result.Ok(ImagesPath);
        }


        protected async Task<Result> SavePropertyImages(Guid propertyId, List<string> images)
        {

            if (!_propertyRepository.IsPropertyExistsById(propertyId))
            {
                return Result.Fail(new NotFoundError("Property", "propertyId", propertyId.ToString(), enApiErrorCode.PropertyNotFound));
            }


            int rowsaffacted = 0;



            foreach (var imgPath in images)
            {

                var img = new PropertyImage
                {
                    PropertyId = propertyId,
                    ImageUrl = imgPath
                };

                if (imgPath == images[0])
                {
                    img.IsMain = true;
                } else
                {
                    img.IsMain = false;
                }
                await _propertyImageRepository.AddAsync(img);

                rowsaffacted += await _propertyImageRepository.SaveChangesAsync();
            };

            if (rowsaffacted != images.Count)
            {
                foreach (var img in images)
                {
                    _fileManager.DeleteFile(img);
                }

                return Result.Fail(new InternalServerError("File", $"Error while saving Property Images", enApiErrorCode.FileSaveError));
            }



            return Result.Ok();
        }
    }
}
