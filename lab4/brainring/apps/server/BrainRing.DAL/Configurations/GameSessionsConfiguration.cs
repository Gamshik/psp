using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
{
    public class GameSessionsConfiguration : IEntityTypeConfiguration<GameSessionEntity>
    {
        public void Configure(EntityTypeBuilder<GameSessionEntity> builder)
        {
            builder.HasKey(gs => gs.Id);
            builder.Property(gs => gs.Id).ValueGeneratedOnAdd();

            builder.Property(gs => gs.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(gs => gs.Host)
                .WithMany(u => u.HostSessions)
                .HasForeignKey(gs => gs.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(gs => gs.CurrentQuestion)
                .WithMany(q => q.CurrentQuestions)
                .HasForeignKey(gs => gs.CurrentQuestionId)
                .OnDelete(DeleteBehavior.SetNull);


            builder.HasMany(gs => gs.Participants)
                .WithOne(p => p.GameSession)
                .HasForeignKey(p => p.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(gs => gs.Questions)
                .WithOne(q => q.GameSession)
                .HasForeignKey(q => q.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
