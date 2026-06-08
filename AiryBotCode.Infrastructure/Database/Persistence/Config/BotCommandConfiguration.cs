using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class BotCommandConfiguration : IEntityTypeConfiguration<BotCommand>
    {
        public void Configure(EntityTypeBuilder<BotCommand> builder)
        {
            builder.ToTable("BotCommands");
            builder.HasKey(bc => bc.Id);
            builder.Property(bc => bc.CommandName).IsRequired();
            // One row per command per bot.
            builder.HasIndex(bc => new { bc.BotId, bc.CommandName }).IsUnique();
        }
    }
}
