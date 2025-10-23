using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
{
    public class GameSessionUsersConfiguration : IEntityTypeConfiguration<GameSessionUserEntity>
    {
        public void Configure(EntityTypeBuilder<GameSessionUserEntity> builder)
        {
            builder.HasKey(gsu => new { gsu.GameSessionId, gsu.UserId });

            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(gsu => gsu.Score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(gsu => gsu.GameSession)
                .WithMany(gs => gs.Participants)
                .HasForeignKey(gsu => gsu.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gsu => gsu.User)
                .WithMany(u => u.GameSessions)
                .HasForeignKey(gsu => gsu.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(gsu => new { gsu.GameSessionId, gsu.UserId }).IsUnique();
        }
    }

}
