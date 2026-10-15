using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class AiryChannelConfiguration : IEntityTypeConfiguration<AiryChannel>
    {
        public void Configure(EntityTypeBuilder<AiryChannel> builder)
        {
            builder.ToTable("AiryChannels");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.AnswerMode).IsRequired();
            builder.HasIndex(c => new { c.BotId, c.ChannelId }).IsUnique();
        }
    }
}
