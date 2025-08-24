using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Statistics.Querys
{
    public class GetMonthlySalesByYearQuery : IRequest<AppResponse<List<MonthlyFinancialSummaryDTO>>>
    {
        public int Year { get; set; }

        public GetMonthlySalesByYearQuery(int year)
        {
            Year = year;
        }
    }

    public class GetMonthlySalesByYearQueryHandler : IRequestHandler<GetMonthlySalesByYearQuery, AppResponse<List<MonthlyFinancialSummaryDTO>>>
    { 
        private readonly ISalesRepository _salesRepository;  

        public GetMonthlySalesByYearQueryHandler(ISalesRepository salesRepository )
        { 
            this._salesRepository = salesRepository;  
        }

        public async Task<AppResponse<List<MonthlyFinancialSummaryDTO>>> Handle(GetMonthlySalesByYearQuery request, CancellationToken cancellationToken)
        { 

            var results = await _salesRepository.GetMonthlySalesByYearAsync(request.Year);

   
            return AppResponse<List<MonthlyFinancialSummaryDTO>>.Success(results);
        }


         

    }
}
