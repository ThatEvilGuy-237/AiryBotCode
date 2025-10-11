using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(m => m.User)
                   .WithMany(u => u.Messages)
                   .HasForeignKey(m => m.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.ChannelConversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ChannelConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(m => m.Context).HasColumnType("TEXT");
            builder.Property(m => m.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
}
