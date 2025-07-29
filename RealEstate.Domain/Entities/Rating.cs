using RealEstate.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Entities;

public partial class Rating : BaseAuditableEntity
{  
    public Guid UserId { get; set; }

    public Guid PropertyId { get; set; }

    public string? RatingText { get; set; }

    public byte RatingNumber { get; set; }

    public virtual Property Property { get; set; } = null!; 

}
