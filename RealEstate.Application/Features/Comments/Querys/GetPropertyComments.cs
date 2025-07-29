using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Comments;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Application.Features.Comments.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Comments.Querys
{
    public class GetPropertyCommentsQuery : IRequest<AppResponse<PaginationResponse<CommentDTO>>>
    {
        public PaginationRequest Pagination { get; }
        public Guid? PropertId { get; }
        public GetPropertyCommentsQuery(PaginationRequest pagination,Guid? propertId) {
            Pagination = pagination;
            PropertId = propertId;
        }

    }


    public class GetPropertyCommentsQueryHandler : IRequestHandler<GetPropertyCommentsQuery, AppResponse<PaginationResponse<CommentDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICommantsRepository _commantsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        public GetPropertyCommentsQueryHandler(
            
            IPropertyRepository propertyRepository,
            ICommantsRepository commantsRepository,
            IUserRepository userRepository,
            IFileManager fileManager,
            IMapper mapper
            )
        {
            this._propertyRepository = propertyRepository;
            this._commantsRepository = commantsRepository;
            this._userRepository = userRepository;
            this._fileManager = fileManager;
            this._mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<CommentDTO>>> Handle(GetPropertyCommentsQuery request, CancellationToken cancellationToken)
        {

            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse<PaginationResponse<CommentDTO>>.Fail(validationResults.Errors);
            }


            var comments = await _commantsRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter: c => c.PropertyId == request.PropertId && !c.IsDeleted,
                orderBy : q => q.OrderByDescending(p => p.CreatedDate)
                );

            // Get total count of properties (after filtering)
            var totalCount = await _commantsRepository.CountAsync();




            var commentsDTO = _mapper.Map<List<CommentDTO>>(comments);
            var commentsDTOWithUsersInfo = await _includeUsersAsync(commentsDTO);

            // Create paginated response
            var paginatedResponse = new PaginationResponse<CommentDTO>
            {
                Items = commentsDTOWithUsersInfo.ToList(),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount 
            }; 
            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<CommentDTO>>.Success(paginatedResponse);
            return response;
        }


        private Result _ValideteRentalData(GetPropertyCommentsQuery request)
        {
            List<Error> errors = new List<Error>();
            if (!request.PropertId.HasValue)
            {
                return Result.Fail(new ValidationError("PropertId", "Propert Id Is Required", enApiErrorCode.RequiredField));
            }
            if (!_propertyRepository.IsPropertyExistsById(request.PropertId.Value))
            {
                errors.Add(new NotFoundError("Property", "PropertyID", request.PropertId.Value.ToString(), enApiErrorCode.UserNotFound));
            } 
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

        private async Task<IEnumerable<CommentDTO>> _includeUsersAsync(IEnumerable<CommentDTO> comments)
        {
            foreach (var item in comments)
            {
                if (Guid.TryParse(item.UserId, out var userId))
                {
                    var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == Guid.Parse(item.UserId), Includes: u => u.Person);

                    if (user != null)
                    {
                        item.FullName = user.Person.FullName;
                        item.Username = user.UserName;
                        item.ImageURL = _fileManager.GetPublicURL(user.Person.ImageURL);
                    }
                }
            }

            return comments;
        }
    }
}
