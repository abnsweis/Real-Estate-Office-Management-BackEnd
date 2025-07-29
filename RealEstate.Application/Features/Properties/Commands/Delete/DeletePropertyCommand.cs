using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Properties.Commands.Delete
{
    public class DeletePropertyCommand : IRequest<AppResponse>
    {
        public Guid PropertyId { get; }

        public DeletePropertyCommand(Guid propertyId) {
            PropertyId = propertyId;
        }

    }



    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, AppResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;

        public DeletePropertyCommandHandler(IPropertyRepository propertyRepository,IFileManager fileManager)
        {
            this._propertyRepository = propertyRepository;
            this._fileManager = fileManager;
        }

        public async Task<AppResponse> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {

            var property =await _propertyRepository.FirstOrDefaultAsync(filter:p=>p.Id== request.PropertyId && !p.IsDeleted,includes:p=>p.PropertyImages);
            if (property == null)
            {
                return AppResponse.Fail(new NotFoundError("Property", "propertyId", request.PropertyId.ToString(), enApiErrorCode.PropertyNotFound));
            }


            _propertyRepository.Delete(property!);

            var error = new InternalServerError("Delete Property", "Failed to delete property", enApiErrorCode.GeneralError);

            var rowsAffacted = await _propertyRepository.SaveChangesAsync();

            if (rowsAffacted > 0)
            {
                foreach (var img in property.PropertyImages)
                {
                    _fileManager.DeleteFile(img.ImageUrl);
                }
                return AppResponse.Success();
            }

            return AppResponse.Fail(error);
        }
    }
}
