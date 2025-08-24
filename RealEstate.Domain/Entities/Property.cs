using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public partial class Property : BaseAuditableEntity
{  
    public string Title { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public Guid CategoryId { get; set; }

    public string PropertyNumber { get; set; } = null!;

    public decimal Price { get; set; }

    public string Location { get; set; } = null!;

    public string Address { get; set; } = null!;

    public PropertyStatus PropertyStatus { get; set; }
    
    public decimal Area { get; set; }

    public string? VideoUrl { get; set; }

    public string Description { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual Customer Owner { get; set; } = null!;

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
