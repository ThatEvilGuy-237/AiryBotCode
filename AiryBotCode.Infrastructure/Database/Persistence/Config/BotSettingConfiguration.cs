using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class BotSettingConfiguration : IEntityTypeConfiguration<BotSetting>
    {
        public void Configure(EntityTypeBuilder<BotSetting> builder)
        {
            builder.ToTable("BotSettings"); // Define the table name

            builder.HasKey(bs => bs.BotId); // BotID is the primary key
            // Configure properties
            builder.Property(bs => bs.BotName)
                .IsRequired()
                .HasMaxLength(255); // Adjust max length as needed

            builder.Property(bs => bs.OpenAIPrompt)
                .IsRequired(false); // Can be null

            builder.Property(bs => bs.Enabled)
                .IsRequired();

            builder.Property(bs => bs.Token)
                .IsRequired(false); // Can be null

            builder.Property(bs => bs.AdminRoleIds)
                .IsRequired(false); // Can be null, stored as string

            builder.Property(bs => bs.EvilId);

            builder.Property(bs => bs.LogChannelId);

            builder.Property(bs => bs.EvilLogChannelId);

            // Values surfaced through the AirySettings registry (all optional).
            builder.Property(bs => bs.OwnerId);
            builder.Property(bs => bs.ListenChannelIds)
                .IsRequired(false); // CSV of channel ids, like AdminRoleIds
            builder.Property(bs => bs.VerifiedRoleId);
            builder.Property(bs => bs.UnverifiedRoleId);
            builder.Property(bs => bs.VerifyLogChannelId);
            builder.Property(bs => bs.RulesChannelId);
            builder.Property(bs => bs.GiveawayScoreboardChannelId);
            builder.Property(bs => bs.ContactCategoryId);
            builder.Property(bs => bs.MaxTokens);
            builder.Property(bs => bs.RetryAttempts);
        }
    }
}
