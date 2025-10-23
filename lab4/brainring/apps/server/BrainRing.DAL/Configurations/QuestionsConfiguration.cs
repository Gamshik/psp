using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
{
    public class QuestionsConfiguration : IEntityTypeConfiguration<QuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionEntity> builder)
        {
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id).ValueGeneratedOnAdd();

            builder.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(q => q.CorrectOptionIndex)
                .IsRequired();

            builder.HasOne(q => q.GameSession)
                .WithMany(gs => gs.Questions)
                .HasForeignKey(q => q.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(q => q.CurrentQuestions)
                .WithOne(gs => gs.CurrentQuestion)
                .HasForeignKey(gs => gs.CurrentQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
