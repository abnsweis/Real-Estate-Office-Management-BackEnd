using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Entities;

public partial class Rental : BaseAuditableEntity
{  
    public Guid LessorId { get; set; }

    public Guid LesseeId { get; set; }

    public Guid PropertyId { get; set; }

    public decimal RentPriceMonth { get; set; }

    public DateOnly StartDate { get; set; } 

    public string Description { get; set; } = null!;

    public string ContractImageUrl { get; set; } = null!;
    public RentType RentType { get; set; }  = RentType.Monthly;
    public short Duration { get; set; }
    public virtual Customer Lessee { get; set; } = null!;

    public virtual Customer Lessor { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public decimal GetTotalPrice()
    {
        int totalMonths = RentType == RentType.Yearly ? Duration * 12 : Duration; 

        return totalMonths * RentPriceMonth;
    }
    
    public DateOnly GetEndDate()
    {
        return RentType == RentType.Monthly ? this.StartDate.AddMonths(Duration) : this.StartDate.AddYears(Duration);
    }

}
