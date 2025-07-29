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

namespace RealEstate.Application.Features.Favorites.Commands.Create
{
    public class AddToFavoritesCommand : IRequest<AppResponse>
    {
        public AddToFavoritesCommand(Guid? propertyId)
        {
            PropertyId = propertyId;
        }

        public Guid? PropertyId { get; }
    }

    public class AddToFavoritesCommandHandler : IRequestHandler<AddToFavoritesCommand, AppResponse>
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _user;

        public AddToFavoritesCommandHandler(
            IFavoritesRepository favoritesRepository,
            IPropertyRepository propertyRepository,
            ICurrentUserService user)
        {
            _favoritesRepository = favoritesRepository;
            _propertyRepository = propertyRepository;
            _user = user;
        }

        public async Task<AppResponse> Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }
            var ExistingFavorite = await _favoritesRepository.FirstOrDefaultAsync(

                filter: f => f.PropertyId == request.PropertyId &&
                f.UserId == _user.UserId );

            var Newfavorite = new Favorite();
            

            


            try
            {
                if (ExistingFavorite != null)
                {
                    Newfavorite = ExistingFavorite;
                    Newfavorite.IsDeleted = false;
                    await _favoritesRepository.UpdateAsync(Newfavorite);
                } else
                {
                    Newfavorite = new Favorite
                    {
                        PropertyId = request.PropertyId!.Value,
                        UserId = _user.UserId!.Value,
                        CreatedDate = DateTimeOffset.UtcNow,
                    };
                    await _favoritesRepository.AddAsync(Newfavorite);
                }
                
                await _favoritesRepository.SaveChangesAsync();
            }
            catch
            {
                return AppResponse.Fail(new InternalServerError("Saving", "failed to save a Comment please try again", enApiErrorCode.GeneralError));
            }
            return AppResponse.Success();
        }


        private Result _ValideteRentalData(AddToFavoritesCommand request)
        {
            List<Error> errors = new List<Error>();

            if (!_user.UserId.HasValue)
            {
                return Result.Fail(new ValidationError("UserID", "User Id Is Required", enApiErrorCode.RequiredField));
            }
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

