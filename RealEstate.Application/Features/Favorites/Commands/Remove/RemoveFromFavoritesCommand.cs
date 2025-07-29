using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Comments.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Favorites.Commands.Remove
{
    public class RemoveFromFavoritesCommand : IRequest<AppResponse>
    {
        public RemoveFromFavoritesCommand(Guid? propertyId)
        {
            PropertyId = propertyId;
        }

        public Guid? PropertyId { get; }
    }

    public class RemoveFromFavoritesCommandHandler : IRequestHandler<RemoveFromFavoritesCommand, AppResponse>
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _user;

        public RemoveFromFavoritesCommandHandler(
            IFavoritesRepository favoritesRepository,
            IPropertyRepository propertyRepository
            ,ICurrentUserService user)
        {
            _favoritesRepository = favoritesRepository;
            _propertyRepository = propertyRepository;
            this._user = user;
        }

        public async Task<AppResponse> Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }

            var favorite = await _favoritesRepository.FirstOrDefaultAsync(
                
                filter: f => f.PropertyId == request.PropertyId &&  
                f.UserId == _user.UserId && 
                !f.IsDeleted);

            if (favorite == null)
            {
                return AppResponse.Fail(new NotFoundError("favorite", "favoriteId","", enApiErrorCode.FavoriteNotFound));
            }



            try
            {
                _favoritesRepository.Delete(favorite);
                await _favoritesRepository.SaveChangesAsync();
            }
            catch
            {
                return AppResponse.Fail(new InternalServerError("Saving", "failed to save a Comment please try again", enApiErrorCode.GeneralError));
            }
            return AppResponse.Success();
        }


        private Result _ValideteRentalData(RemoveFromFavoritesCommand request)
        {
            List<Error> errors = new List<Error>();

 
            if (!request.PropertyId.HasValue)
            {
                return Result.Fail(new ValidationError("PropertId", "Propert Id Is Required", enApiErrorCode.RequiredField));
            }
            if (!_propertyRepository.IsPropertyExistsById(request.PropertyId.Value))
            {
                errors.Add(new NotFoundError("Property", "PropertyID", request.PropertyId.Value.ToString(), enApiErrorCode.PropertyNotFound));
            }
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}

