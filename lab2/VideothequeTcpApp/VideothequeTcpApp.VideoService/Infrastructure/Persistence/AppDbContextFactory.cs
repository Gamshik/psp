using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VideothequeTcpApp.VideoService.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Указываем SQLite
            optionsBuilder.UseSqlite("Data Source=D:\\ProgrammingAndProjects\\Studies\\7sem\\psp\\lab2\\VideothequeTcpApp\\VideothequeTcpApp.VideoService\\videotheque.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }

}
