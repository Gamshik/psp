using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainRing.DbAdapter
{
    public class BrainRingDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GameSessionEntity> GameSessions { get; set; }
        public DbSet<QuestionEntity> Questions { get; set; }
        public DbSet<GameSessionUserEntity> GameSessionUsers { get; set; }

        public BrainRingDbContext(DbContextOptions<BrainRingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrainRingDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
