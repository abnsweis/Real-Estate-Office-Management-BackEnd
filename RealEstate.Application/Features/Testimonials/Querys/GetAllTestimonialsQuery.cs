using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Testimonials.Querys
{

    public class GetAllTestimonialsQuery : IRequest<AppResponse<PaginationResponse<TestimonialDTO>>>
    {
 
        public PaginationRequest Pagination { get; set; }

 
        public GetAllTestimonialsQuery(PaginationRequest pagination)
        {
            Pagination = pagination;
        }
    }
 
    public class GetAllTestimonialsQueryHandler : IRequestHandler<GetAllTestimonialsQuery, AppResponse<PaginationResponse<TestimonialDTO>>>
    {

        private readonly ITestimonialsRepository _testimonialsRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
 
        public GetAllTestimonialsQueryHandler(
            ITestimonialsRepository TestimonialsRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _testimonialsRepository = TestimonialsRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<TestimonialDTO>>> Handle(GetAllTestimonialsQuery request, CancellationToken cancellationToken)
        {


            // Retrieve Testimonials from repository with pagination, and includes
            var Testimonials = await _testimonialsRepository.GetAllWithUserNamesAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize );

            // Get total count of properties (after filtering)
            var totalCount = await _testimonialsRepository.CountAsync();






            // Create paginated response
            var paginatedResponse = new PaginationResponse<TestimonialDTO>
            {
                Items = _mapper.Map<List<TestimonialDTO>>(Testimonials),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
            // Process media URLs for each Testimonial
            foreach (var item in paginatedResponse.Items)
            {  
                // Convert video path to public URL if exists
                if (item.ImageURL is not null)
                {
                    item.ImageURL = _fileManager.GetPublicURL(item.ImageURL);
                }
            }
            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<TestimonialDTO>>.Success(paginatedResponse);
            return response;
        }
    }
}