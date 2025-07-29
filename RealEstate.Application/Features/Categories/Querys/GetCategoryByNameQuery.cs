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
    public class GetCategoryByNameQuery : IRequest<AppResponse<CategoryDTO>>
    {
        public string? CategoryName { get; }
        public GetCategoryByNameQuery(string? categoryName) => CategoryName = categoryName;

    }

    public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, AppResponse<CategoryDTO>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryByNameQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<AppResponse<CategoryDTO>> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(filter: category => category.CategoryName == request.CategoryName && !category.IsDeleted);

            if (category is null)
            {
                return new AppResponse<CategoryDTO>
                {
                    Result = Result.Fail(new NotFoundError("category", "categoryName", request.CategoryName.ToString(), enApiErrorCode.CategoryNotFound))
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
