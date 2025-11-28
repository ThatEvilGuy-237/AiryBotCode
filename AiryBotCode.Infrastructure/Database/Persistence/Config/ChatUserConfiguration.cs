using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
    {
        public void Configure(EntityTypeBuilder<ChatUser> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserName).HasMaxLength(255).IsRequired();
        }
    }
}
