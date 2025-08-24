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
    public class GetStatisticsQuery : IRequest<AppResponse<List<StatisticDTO>>>
    {
    }

    public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, AppResponse<List<StatisticDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ISalesRepository _salesRepository; 
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRentalsRepository _rentalsRepository;

        public GetStatisticsQueryHandler(
            
            IPropertyRepository propertyRepository,
            ISalesRepository salesRepository, 
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRentalsRepository rentalsRepository

            )
        {
            this._propertyRepository = propertyRepository;
            this._salesRepository = salesRepository; 
            this._userRepository = userRepository;
            this._customerRepository = customerRepository;
            this._rentalsRepository = rentalsRepository;
        }

        public async Task<AppResponse<List<StatisticDTO>>> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            decimal MonthlySales = _salesRepository.GetMonthlySales();
            decimal previousMonthlySales = _salesRepository.GetMonthlySales(DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1);

            decimal MonthlyRental = _rentalsRepository.GetMonthlyRental();
            decimal previousMonthlyRental = _rentalsRepository.GetMonthlyRental(DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1);

            int customersCount = await _customerRepository.CountAsync();
            int customersCountInCurrentMonth = await _customerRepository.CountCreatedInCurrentMonth();

            int usersCount = await _userRepository.CountAsync();
            int adminsCount = await _userRepository.AdminsCountAsync();

            int availableProperties = await _propertyRepository.CountAsync(p => p.PropertyStatus == Domain.Enums.PropertyStatus.Available);
            int rentedProperties = await _propertyRepository.CountAsync(p => p.PropertyStatus == Domain.Enums.PropertyStatus.Rented);
            int soldProperties = await _propertyRepository.CountAsync(p => p.PropertyStatus == Domain.Enums.PropertyStatus.Sold);

             


        var stats = new List<StatisticDTO>
        {
            new StatisticDTO
            {
                Title = "المبيعات الشهرية",
                Value = $"${MonthlySales:N0}",
                Color = "text-primary",
                Description = $"{CalcPercentChange(MonthlySales, previousMonthlySales)} مقارنة بالشهر الماضي"
            },
            new StatisticDTO
            {
                Title = "الإيجارات الشهرية",
                Value = $"${MonthlyRental:N0}",
                Color = "text-success",
                Description = $"{CalcPercentChange(MonthlyRental, previousMonthlyRental)} مقارنة بالشهر الماضي"
            },
            new StatisticDTO
            {
                Title = "العملاء",
                Value = $"{customersCount} عميل",
                Color = "text-info",
                Description = $"+{customersCountInCurrentMonth} عميل جديد هذا الشهر"
            },
            new StatisticDTO
            {
                Title = "المستخدمين",
                Value = $"{usersCount} مستخدم",
                Color = "text-warning",
                Description = $"بينهم {adminsCount} مشرف"
            },
            new StatisticDTO
            {
                Title = "العقارات المتاحة",
                Value = $"{availableProperties} عقار",
                Color = "text-success",
                Description = "محدثة حتى الآن"
            },
            new StatisticDTO
            {
                Title = "العقارات المؤجرة",
                Value = $"{rentedProperties} عقار",
                Color = "text-primary",
                Description = "بأنواع إيجار مختلفة"
            },
            new StatisticDTO
            {
                Title = "العقارات المباعة",
                Value = $"{soldProperties} عقار",
                Color = "text-danger",
                Description = "+2 بيع جديد هذا الشهر" // ممكن تحسب الفرق وتعرضه برضه
            },
            new StatisticDTO
            {
                Title = "طلبات قيد المراجعة",
                Value = "12 طلب", // إذا عندك عدد فعلي من الريبو حطه هنا
                Color = "text-warning",
                Description = "بعضها من عملاء جدد"
            }
        };

            return AppResponse<List<StatisticDTO>>.Success(stats);
        }


        string CalcPercentChange(decimal current, decimal previous)
        {
            if (previous == 0) return "+100%";
            var diff = current - previous;
            var percent = (diff / previous) * 100;
            return (percent >= 0 ? "+" : "") + $"{percent:F0}%";
        }

    }
}
