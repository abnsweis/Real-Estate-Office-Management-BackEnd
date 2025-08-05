using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Features.Statistics.Querys
{
    public class GetTotalRentalsRevenueQuery : IRequest<AppResponse<decimal>>
    { 

        public GetTotalRentalsRevenueQuery()
        {
           
        }
    }

    public class GetTotalRentalsRevenueQueryHandler : IRequestHandler<GetTotalRentalsRevenueQuery, AppResponse<decimal>>
    { 
        private readonly IRentalsRepository _rentalsRepository;  

        public GetTotalRentalsRevenueQueryHandler(IRentalsRepository rentalsRepository)
        { 
            this._rentalsRepository = rentalsRepository;  
        }

        public Task<AppResponse<decimal>> Handle(GetTotalRentalsRevenueQuery request, CancellationToken cancellationToken)
        {
            var result = _rentalsRepository.GetTotalRentalsRevenue();
            return Task.FromResult(AppResponse<decimal>.Success(result));
        }
    }
}
