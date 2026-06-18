using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class LevelUserConfiguration : IEntityTypeConfiguration<LevelUser>
    {
        public void Configure(EntityTypeBuilder<LevelUser> builder)
        {
            builder.ToTable("LevelUsers");
            builder.HasKey(x => x.Id);
            // One row per bot+user — also the lookup index.
            builder.HasIndex(x => new { x.BotId, x.UserId }).IsUnique();
        }
    }
}
