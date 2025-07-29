using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Dtos.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Commands.Update.UpdateUserImage
{
    public class UpdateUserImageCommand : IRequest<AppResponse>
    {
        public Guid UserId { get; init; }
        public IFormFile? ImageFile { get; set; }

        public UpdateUserImageCommand(Guid userId, IFormFile imageFile)
        {
            UserId = userId;
            ImageFile = imageFile;
        }
    }
}
