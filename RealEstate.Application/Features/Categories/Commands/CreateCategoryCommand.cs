using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<AppResponse<Guid>>{
        public CreateUpdateCategoryDTO CategoryData { get; }
        public CreateCategoryCommand(CreateUpdateCategoryDTO categoryDate) => CategoryData = categoryDate;
    }



    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, AppResponse<Guid>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<AppResponse<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.CategoryData.CategoryName))
            {
                return AppResponse<Guid>.Fail(new ValidationError("CategoryName", "Category Name Is Required", enApiErrorCode.MissingCategoryName));   
            }

            if (request.CategoryData.CategoryName.Length < 2 || request.CategoryData.CategoryName.Length > 100)
            {
                return AppResponse<Guid>.Fail(new ValidationError("CategoryName", "Category name must be between 3 and 99 characters.", enApiErrorCode.MissingCategoryName));
            }
            if (await _categoryRepository.FirstOrDefaultAsync(category => category.CategoryName == request.CategoryData.CategoryName) != null)
            {
                return AppResponse<Guid>.Fail(new ConflictError("Category", "Category Name Already Exists", enApiErrorCode.CategoryNameAlreadyExists));

            }

            var NewCategory = new Category
            {
                CategoryName = request.CategoryData.CategoryName
            };


            await _categoryRepository.AddAsync(NewCategory);
            await _categoryRepository.SaveChangesAsync();


            return AppResponse<Guid>.Success(NewCategory.Id);
        }
    }
}
