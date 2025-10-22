using BrainRing.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DAL.Configurations
{
    public class UsersConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Name).IsRequired();

            builder.HasMany(u => u.GameSessions)
                .WithOne(gs => gs.User)
                .HasForeignKey(gs => gs.UserId);

            builder.HasMany(u => u.HostSessions)
                .WithOne(hs => hs.Host)
                .HasForeignKey(hs => hs.HostId);

            builder.HasMany(u => u.Answers)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        }
    }
}
