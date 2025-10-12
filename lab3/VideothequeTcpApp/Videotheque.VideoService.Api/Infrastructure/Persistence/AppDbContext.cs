using Microsoft.EntityFrameworkCore;
using Videotheque.VideoService.Api.Models.Domain;

namespace Videotheque.VideoService.Api.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Video> Videos => Set<Video>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite("Data Source=D:\\ProgrammingAndProjects\\Studies\\7sem\\psp\\lab3\\VideothequeTcpApp\\Videotheque.VideoService.Api\\video.db");
        }
    }

}
