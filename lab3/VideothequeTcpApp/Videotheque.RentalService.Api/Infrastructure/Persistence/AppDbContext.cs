using Microsoft.EntityFrameworkCore;
using Videotheque.RentalService.Api.Models.Domain;

namespace Videotheque.RentalService.Api.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Rental> Rentals => Set<Rental>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite("Data Source=D:\\ProgrammingAndProjects\\Studies\\7sem\\psp\\lab3\\VideothequeTcpApp\\Videotheque.RentalService.Api\\rentals.db");
        }
    }
}
