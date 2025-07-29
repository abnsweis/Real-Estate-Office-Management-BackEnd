using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Comments
{
    public class UpdateCommentDto
    {
        public Guid? CommentId { get; set; }
        public string? Text { get; set; }
    }
}
