using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class SuggestionConfiguration : IEntityTypeConfiguration<Suggestion>
    {
        public void Configure(EntityTypeBuilder<Suggestion> builder)
        {
            builder.ToTable("Suggestions");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired();
            builder.Property(s => s.Body).IsRequired();
            builder.Property(s => s.Status).IsRequired();
        }
    }
}
