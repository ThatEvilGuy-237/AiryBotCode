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

            builder.Property(bs => bs.Enabled)
                .IsRequired();

            builder.Property(bs => bs.Token)
                .IsRequired(false); // Can be null

            builder.Property(bs => bs.AdminRoleIds)
                .IsRequired(false); // Can be null, stored as string

            builder.Property(bs => bs.EvilId);

            builder.Property(bs => bs.LogChannelId);

            builder.Property(bs => bs.EvilLogChannelId);

            builder.Property(bs => bs.DatabaseName)
                .IsRequired(false); // null/empty = shared default database

            builder.Property(bs => bs.ThemePrimary).IsRequired(false);
            builder.Property(bs => bs.ThemeAccent).IsRequired(false);
            builder.Property(bs => bs.ThemeImage).IsRequired(false);
            builder.Property(bs => bs.ThemeData).IsRequired(false);


        }
    }
}
