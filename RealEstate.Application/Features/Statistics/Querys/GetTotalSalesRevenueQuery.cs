using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Features.Statistics.Querys
{
    public class GetTotalSalesRevenueQuery : IRequest<AppResponse<decimal>>
    { 

        public GetTotalSalesRevenueQuery()
        {
           
        }
    }

    public class GetTotalSalesRevenueQueryHandler : IRequestHandler<GetTotalSalesRevenueQuery, AppResponse<decimal>>
    { 
        private readonly ISalesRepository _salesRepository;  

        public GetTotalSalesRevenueQueryHandler(ISalesRepository salesRepository)
        { 
            this._salesRepository = salesRepository;  
        }

        public Task<AppResponse<decimal>> Handle(GetTotalSalesRevenueQuery request, CancellationToken cancellationToken)
        {
            var result = _salesRepository.GetTotalSalesRevenue();
            return Task.FromResult(AppResponse<decimal>.Success(result));
        }
    }
}
