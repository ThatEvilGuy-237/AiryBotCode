using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class ChannelDecisionWatchConfiguration : IEntityTypeConfiguration<ChannelDecisionWatch>
    {
        public void Configure(EntityTypeBuilder<ChannelDecisionWatch> builder)
        {
            builder.ToTable("ChannelDecisionWatches");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name).IsRequired();
            builder.Property(w => w.ListenerUrl).IsRequired();
            builder.Property(w => w.ActionsJson).IsRequired();
            builder.Property(w => w.UnsureAction).IsRequired();
            builder.HasIndex(w => new { w.BotId, w.ChannelId });
        }
    }
}
