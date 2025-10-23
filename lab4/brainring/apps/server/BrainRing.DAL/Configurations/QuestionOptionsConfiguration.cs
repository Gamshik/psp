using BrainRing.DbAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainRing.DbAdapter.Configurations
{
    public class QuestionOptionsConfiguration : IEntityTypeConfiguration<QuestionOptionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionOptionEntity> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
