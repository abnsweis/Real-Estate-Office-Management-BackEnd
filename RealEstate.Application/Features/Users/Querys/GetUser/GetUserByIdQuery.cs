using FluentResults;
using MediatR;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Querys.GetUser
{
    public class GetUserByIdQuery : IRequest<Result<UserDTO>>
    {
        public Guid Id { get; }
        public GetUserByIdQuery(Guid Id) => this.Id = Id;
    }
    public class GetUserCountQuery : IRequest<int> { };



}
