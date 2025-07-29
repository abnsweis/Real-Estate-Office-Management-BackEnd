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
    public class IsInFavoriteQuery : IRequest<AppResponse<bool>>
    {  
        public IsInFavoriteQuery(Guid propertyId)
        {
            PropertyId = propertyId;
        }

        public Guid PropertyId { get; }
    }

    public class IsInFavoriteQueryQueryHandler : IRequestHandler<IsInFavoriteQuery, AppResponse<bool>>
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly ICurrentUserService _user;

        public IsInFavoriteQueryQueryHandler( 
            IFavoritesRepository favoritesRepository,
            ICurrentUserService user
            )
        {
            this._favoritesRepository = favoritesRepository;
            this._user = user;
        }

        public async Task<AppResponse<bool>> Handle(IsInFavoriteQuery request, CancellationToken cancellationToken)
        {
                    
            var IsInFavorite = _favoritesRepository.IsInFavorite(request.PropertyId, _user.UserId.Value);

            if (!IsInFavorite)
            {
                return AppResponse<bool>.Success(false);
            }  
            return AppResponse<bool>.Success(true);
        }

         
    }
}
