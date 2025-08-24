using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Categories.Querys
{
    public class GetAllCategoriesQuery : IRequest<AppResponse<PaginationResponse<CategoryDTO>>>
    {
        public PaginationRequest Pagination { get; set; }
        public string? CategoryNameFilter { get; set; }

        public GetAllCategoriesQuery(PaginationRequest pagination, string? categoryNameFilter)
        {
            Pagination = pagination;
            CategoryNameFilter = categoryNameFilter;
        }
    }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, AppResponse<PaginationResponse<CategoryDTO>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<CategoryDTO>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Category, bool>> filter = category =>
                (string.IsNullOrEmpty(request.CategoryNameFilter) || category.CategoryName.StartsWith(request.CategoryNameFilter))
                && category.IsDeleted == false;

            var categories = await _categoryRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter,
                includes : c => c.Properties,
                orderBy : q => q.OrderBy(category => category.CategoryName)
            );

            var categoriesDTO = categories.Select(category => new CategoryDTO { 
                CategoryId = category.Id.ToString(),
                CategoryName = category.CategoryName ,
                PropertiesCount = category.Properties.Count.ToString()
            }).ToList();

            var totalCount = await _categoryRepository.CountAsync();

            var paginatedResponse = new PaginationResponse<CategoryDTO>
            {
                Items = categoriesDTO,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
            var response = AppResponse<PaginationResponse<CategoryDTO>>.Success(paginatedResponse);
            return response; 
        }
    }
}
