using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class ChannelWebhookConfiguration : IEntityTypeConfiguration<ChannelWebhook>
    {
        public void Configure(EntityTypeBuilder<ChannelWebhook> builder)
        {
            builder.ToTable("ChannelWebhooks");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.WebhookUrl).IsRequired();
            // Fast lookup of "is this channel linked for this bot".
            builder.HasIndex(c => new { c.BotId, c.ChannelId });
        }
    }
}
