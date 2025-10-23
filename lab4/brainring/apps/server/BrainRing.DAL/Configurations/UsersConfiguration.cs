using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
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
                .HasForeignKey(gs => gs.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.HostSessions)
                .WithOne(hs => hs.Host)
                .HasForeignKey(hs => hs.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Answers)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
