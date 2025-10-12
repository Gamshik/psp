using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Videotheque.VideoService.Api.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=D:\\ProgrammingAndProjects\\Studies\\7sem\\psp\\lab3\\VideothequeTcpApp\\Videotheque.VideoService.Api\\video.db");
            return new AppDbContext(optionsBuilder.Options);
        }
    }

}
