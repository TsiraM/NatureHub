using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NatureHub.Models;

namespace NatureHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for your models
        public DbSet<Discussion> Discussions { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
    }
}
