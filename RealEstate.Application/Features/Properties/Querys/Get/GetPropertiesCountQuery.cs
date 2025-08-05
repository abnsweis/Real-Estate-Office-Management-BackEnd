using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Features.Properties.Querys.Get
{
     
    public class GetPropertiesCountQuery : IRequest<int>
    {
        
    }
     
    public class GetPropertiesCountQueryHndler : IRequestHandler<GetPropertiesCountQuery, int>
    {
        private readonly IPropertyRepository _propertyRepository;
 
         
        public GetPropertiesCountQueryHndler(
            IPropertyRepository propertyRepository )
        {
            _propertyRepository = propertyRepository;
    
        }

        public async Task<int> Handle(GetPropertiesCountQuery request, CancellationToken cancellationToken)
        {
            return  await _propertyRepository.CountAsync();
        }
    }
}