using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Categories.Commands.NewFolder
{
    public class UpdateCategoryCommand : IRequest<AppResponse>
    {
        public Guid CategoryId { get; set; }  
        public CreateUpdateCategoryDTO CategoryData { get; set; }

        public UpdateCategoryCommand(Guid categoryid, CreateUpdateCategoryDTO categoryData)
        {
            CategoryId = categoryid;
            CategoryData = categoryData;
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, AppResponse>
    {
        private readonly ICategoryRepository _categoryRepository; 

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository; 
        }
        public async Task<AppResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {

            var categoryToUpdate = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == request.CategoryId && !c.IsDeleted);

            if (categoryToUpdate == null)
            {
                return AppResponse.Fail(new NotFoundError("category", "categoryId", nameof(request.CategoryId), enApiErrorCode.CategoryNotFound));
            }
            if (string.IsNullOrEmpty(request.CategoryData.CategoryName))
            {
                return AppResponse.Fail(new ValidationError("CategoryName", "Category Name Is Required", enApiErrorCode.MissingCategoryName));
            }

            if (request.CategoryData.CategoryName.Length < 2 || request.CategoryData.CategoryName.Length > 100)
            {
                return AppResponse.Fail(new ValidationError("CategoryName", "Category name must be between 3 and 99 characters.", enApiErrorCode.MissingCategoryName));
            }
            if (_categoryRepository.IsCategoryExists(request.CategoryData.CategoryName) && request.CategoryData.CategoryName != categoryToUpdate!.CategoryName)
            {
                return AppResponse.Fail(new ConflictError("Category", "Category Name Already Exists", enApiErrorCode.CategoryNameAlreadyExists));
            }


            categoryToUpdate.CategoryName = request.CategoryData.CategoryName;

            await _categoryRepository.UpdateAsync(categoryToUpdate);
            await _categoryRepository.SaveChangesAsync();
            

            return AppResponse.Success(); 
        }
    }
}
