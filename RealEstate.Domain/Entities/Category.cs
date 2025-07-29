using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class Category : BaseAuditableEntity
{  
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
