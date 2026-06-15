using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
    {
        public void Configure(EntityTypeBuilder<UserConsent> builder)
        {
            builder.ToTable("UserConsents");
            builder.HasKey(c => c.Id);
            // One consent row per bot+user — also the lookup index.
            builder.HasIndex(c => new { c.BotId, c.UserId }).IsUnique();
        }
    }
}
