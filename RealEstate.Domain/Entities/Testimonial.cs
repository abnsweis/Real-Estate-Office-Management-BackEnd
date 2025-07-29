using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class Testimonial : BaseEntity
{  
    public Guid UserId { get; set; }

    public string? RatingText { get; set; }

    public byte RatingNumber { get; set; }
}
    