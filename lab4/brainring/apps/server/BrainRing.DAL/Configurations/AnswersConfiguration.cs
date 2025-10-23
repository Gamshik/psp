using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
{
    public class AnswersConfiguration : IEntityTypeConfiguration<AnswerEntity>
    {
        public void Configure(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.SelectedOptionIndex)
                .IsRequired();

            builder.Property(a => a.IsCorrect)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => new { a.QuestionId, a.UserId }).IsUnique();
        }
    }
}
