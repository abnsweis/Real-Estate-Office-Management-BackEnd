using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class PropertyImage : BaseAuditableEntity
{  
    public Guid PropertyId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public virtual Property Property { get; set; } = null!;
}
