using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class SuggestionShareCodeConfiguration : IEntityTypeConfiguration<SuggestionShareCode>
    {
        public void Configure(EntityTypeBuilder<SuggestionShareCode> builder)
        {
            builder.ToTable("SuggestionShareCodes");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Code).IsRequired();
        }
    }
}
