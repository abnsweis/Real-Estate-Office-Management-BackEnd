using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class Sale : BaseAuditableEntity
{  
    public Guid SellerId { get; set; }

    public Guid BuyerId { get; set; }

    public Guid PropertyId { get; set; }

    public decimal? Price { get; set; }

    public DateOnly SaleDate { get; set; }

    public string Description { get; set; } = null!;

    public string ContractImageUrl { get; set; } = null!;

    public virtual Customer Buyer { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public virtual Customer Seller { get; set; } = null!;
}
