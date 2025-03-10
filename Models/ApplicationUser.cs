using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatureHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? ImageFilename { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
    }
}