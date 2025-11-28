using AiryBotCode.Domain.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryBotCode.Infrastructure.Database.Persistence.Config
{
    public class GiveAwayUserConfiguration : IEntityTypeConfiguration<GiveAwayUser>
    {
        public void Configure(EntityTypeBuilder<GiveAwayUser> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserName).HasMaxLength(255).IsRequired();
        }
    }
}
