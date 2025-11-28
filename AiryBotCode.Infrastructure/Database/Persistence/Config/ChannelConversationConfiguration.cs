using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class ChannelConversationConfiguration : IEntityTypeConfiguration<ChannelConversation>
    {
        public void Configure(EntityTypeBuilder<ChannelConversation> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.ChannelId).IsRequired();
            builder.Property(c => c.ConversationSummary).HasColumnType("TEXT");
        }
    }
}
