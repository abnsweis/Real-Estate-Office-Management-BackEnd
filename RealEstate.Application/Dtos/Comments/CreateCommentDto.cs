using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Comments
{
    public class CreateCommentDto
    {
        public Guid? PropertyId { get; set; }
        public string? Text { get; set; }
    }
}
