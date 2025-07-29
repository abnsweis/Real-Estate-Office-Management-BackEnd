using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Favorites;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Favorites.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Favorites.Querys
{
    public class GetUserFavoritesQuery : IRequest<AppResponse<PaginationResponse<FavoriteDTO>>>
    { 
        public GetUserFavoritesQuery(PaginationRequest pagination)
        {
            Pagination = pagination;
        }

        public PaginationRequest Pagination { get; }
    }

    public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, AppResponse<PaginationResponse<FavoriteDTO>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly ICurrentUserService _user;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public GetUserFavoritesQueryHandler(
            IUserRepository userRepository,
            IFavoritesRepository favoritesRepository,
            ICurrentUserService user,
            IMapper mapper,
            IFileManager fileManager
            )
        {
            this._userRepository = userRepository;
            this._favoritesRepository = favoritesRepository;
            this._user = user;
            this._mapper = mapper;
            this._fileManager = fileManager;
        }

        public async Task<AppResponse<PaginationResponse<FavoriteDTO>>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
        {

            var validationResults = _ValideteData();

            if (validationResults.IsFailed)
            {
                return AppResponse<PaginationResponse<FavoriteDTO>>.Fail(validationResults.Errors);
            }

            var propertis = await _favoritesRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize, 
                filter : f => f.UserId == _user.UserId && !f.IsDeleted,
                includes : new Expression<Func<Favorite, object>>[]
                {
                    f => f.Property,
                    f => f.Property.PropertyImages,
                }
                );

            var totalCount = await _favoritesRepository.CountAsync();

            var propertisDTO = _mapper.Map<List<FavoriteDTO>>(propertis);

            foreach (var item in propertisDTO)
            {
                if (item.MainImage != null)
                {
                    item.MainImage = _fileManager.GetPublicURL(item.MainImage);
                }
            }

            var paginatedResponse = new PaginationResponse<FavoriteDTO>
            {
                Items = propertisDTO,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };

            
            var response = AppResponse<PaginationResponse<FavoriteDTO>>.Success(paginatedResponse);
            return response;
            throw new NotImplementedException();
        }

        private Result _ValideteData()
        {
            List<Error> errors = new List<Error>();

            if (!_user.UserId.HasValue)
            {
                return Result.Fail(new ValidationError("UserID", "User Id Is Required", enApiErrorCode.RequiredField));
            }
            if (!_userRepository.IsUserExists(_user.UserId.Value))
            {
                return Result.Fail(new NotFoundError("User", "UserID", _user.UserId.Value.ToString(), enApiErrorCode.UserNotFound));
            } 
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}
