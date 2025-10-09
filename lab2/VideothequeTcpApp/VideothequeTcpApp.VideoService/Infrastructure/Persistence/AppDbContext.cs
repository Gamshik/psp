using Microsoft.EntityFrameworkCore;
using VideothequeTcpApp.Models.Domain;

namespace VideothequeTcpApp.VideoService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Video>().HasData(
                new Video
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    Year = 2010,
                    Director = "Christopher Nolan",
                    Price = 4.99m
                },
                new Video
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "Interstellar",
                    Genre = "Sci-Fi",
                    Year = 2014,
                    Director = "Christopher Nolan",
                    Price = 5.49m
                },
                new Video
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Title = "The Matrix",
                    Genre = "Action",
                    Year = 1999,
                    Director = "Lana & Lilly Wachowski",
                    Price = 3.99m
                }
            );
        }
    }
}
