using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class Favorite  : BaseEntity
{ 
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid PropertyId { get; set; }
    public virtual Property Property { get; set; } = null!;
}
