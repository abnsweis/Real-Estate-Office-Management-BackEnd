using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace RealEstate.Application.Features.Users.Querys.ReadAll
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginationResponse<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;
        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _fileManager = fileManager;
        }

        public async Task<PaginationResponse<UserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {

            Expression<Func<UserDomain, bool>> filter = user =>
                (string.IsNullOrEmpty(request.Filtter.FullName) || user.Person.FullName.StartsWith(request.Filtter.FullName)) &&
                (string.IsNullOrEmpty(request.Filtter.Email) || user.Email == request.Filtter.Email) &&
                (string.IsNullOrEmpty(request.Filtter.Username) || user.UserName == request.Filtter.Username) &&
                (string.IsNullOrEmpty(request.Filtter.Phone) || user.PhoneNumber == request.Filtter.Phone);



            var users = await _userRepository.GetAllAsync(request.pagination.PageNumber, request.pagination.PageSize, filter);

            foreach (var user in users)
            {
                user.Person.ImageURL = _fileManager.GetPublicURL(user.Person.ImageURL);
            }

            var totalCount = await _userRepository.CountAsync();
            var response = new PaginationResponse<UserDTO>
            {
                Items = _mapper.Map<List<UserDTO>>(users),
                PageNumber = request.pagination.PageNumber,
                PageSize = request.pagination.PageSize,
                TotalCount = totalCount

            };
            return response;
        }
    }
}
