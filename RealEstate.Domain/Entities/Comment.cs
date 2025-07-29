using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public partial class Comment : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid PropertyId { get; set; }

    public string CommentText { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; }
    public virtual Property Property { get; set; } = null!;
}
