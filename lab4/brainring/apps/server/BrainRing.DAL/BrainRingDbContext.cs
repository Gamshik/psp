using BrainRing.DAL.Configurations;
using BrainRing.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainRing.DAL
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UsersConfiguration());
        }
    }
}
