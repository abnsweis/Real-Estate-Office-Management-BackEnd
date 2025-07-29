using MediatR;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Querys.ReadAll
{
    public class GetAllUsersQuery : IRequest<PaginationResponse<UserDTO>>
    {
        public PaginationRequest pagination;
        public FiltterUserDTO Filtter { get; }

        public GetAllUsersQuery(PaginationRequest pagination, FiltterUserDTO filtter)
        {
            this.pagination = pagination;
            Filtter = filtter;
        }

    }
}
