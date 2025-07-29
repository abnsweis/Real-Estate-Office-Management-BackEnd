using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Comments
{
    public class CommentDTO
    {
        public string? CommentID { get; set; } 
        public string? PropertyId    { get; set; } 
        public string? UserId    { get; set; } 
        public string? Username    { get; set; } 
        public string? FullName { get; set; } 
        public string? CommentText { get; set; } 
        public string? ImageURL { get; set; } 
        public string? CreatedDate { get; set; }  
    }
}
