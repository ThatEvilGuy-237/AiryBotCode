using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class CommandSettingConfiguration : IEntityTypeConfiguration<CommandSetting>
    {
        public void Configure(EntityTypeBuilder<CommandSetting> builder)
        {
            builder.HasKey(cs => cs.Id); // Assuming 'Id' is the primary key
            builder.HasIndex(cs => new { cs.CommandName, cs.Key })
                   .IsUnique();
        }
    }
}
