using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class CountingStateConfiguration : IEntityTypeConfiguration<CountingState>
    {
        public void Configure(EntityTypeBuilder<CountingState> builder)
        {
            builder.ToTable("CountingStates");
            builder.HasKey(x => x.Id);
            // One row per bot+channel — also the lookup index.
            builder.HasIndex(x => new { x.BotId, x.ChannelId }).IsUnique();
        }
    }
}
