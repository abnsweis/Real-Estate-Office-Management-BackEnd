using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Statistics;
using System.Threading;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Statistics.Querys
{ 
    public class GetSalesByMonthQuery : IRequest<AppResponse<MonthlyFinancialSummaryDTO>>
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public GetSalesByMonthQuery(int year, int month)
        {
            Year = year;
            Month = month;
        }
    }
     
    public class GetSalesByMonthQueryHandler : IRequestHandler<GetSalesByMonthQuery, AppResponse<MonthlyFinancialSummaryDTO>>
    {
        private readonly ISalesRepository _salesRepository;

        public GetSalesByMonthQueryHandler(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<AppResponse<MonthlyFinancialSummaryDTO>> Handle(GetSalesByMonthQuery request, CancellationToken cancellationToken)
        {
            var result = await _salesRepository.GetSalesByMonthAsync(request.Year, request.Month);

            return AppResponse<MonthlyFinancialSummaryDTO>.Success(result);
        }
    }
}
