using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public partial class Customer : BaseAuditableEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public enCustomerType CustomerType { get; set; } 
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<Rental> RentalLessees { get; set; } = new List<Rental>();

    public virtual ICollection<Rental> RentalLessors { get; set; } = new List<Rental>();

    public virtual ICollection<Sale> SaleBuyers { get; set; } = new List<Sale>();

    public virtual ICollection<Sale> SaleSellers { get; set; } = new List<Sale>();

    
}
