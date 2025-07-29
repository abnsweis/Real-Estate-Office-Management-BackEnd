using AutoMapper;
using FluentResults;
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

namespace RealEstate.Application.Features.Categories.Querys
{
    public class GetCategoryByIdQuery : IRequest<AppResponse<CategoryDTO>>
    {
        public Guid CategoryId { get; }
        public GetCategoryByIdQuery(Guid categoryId) => CategoryId = categoryId;

    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, AppResponse<CategoryDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<AppResponse<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(filter: category => category.Id == request.CategoryId && !category.IsDeleted);

            if (category is null)
            {
                return new AppResponse<CategoryDTO>
                {
                    Result = Result.Fail(new NotFoundError("category", "categoryId", request.CategoryId.ToString(), enApiErrorCode.CategoryNotFound))
                };
            }

            return AppResponse<CategoryDTO>.Success(
                new CategoryDTO { 
                    CategoryId = category.Id.ToString(),
                    CategoryName = category.CategoryName
                }
            );
        }
    }
}
