using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Database.Persistence
{
    public class AIDbContext : DbContext
    {
        public AIDbContext(DbContextOptions<AIDbContext> options) : base(options) {
        }

        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChannelConversation> ChannelConversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ChatUserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConversationConfiguration());
        }


        public static IServiceCollection registerDbContext(IServiceCollection services)
        {
            IConfigurationReader _config = services.BuildServiceProvider().GetRequiredService<IConfigurationReader>();
            string connectionString = _config.GetDatabaseConnectionString();
            services.AddDbContext<AIDbContext>(options =>
                options.UseNpgsql(connectionString));

            Console.WriteLine($"[DATABASE] Connecting to database... '{string.Join(";", connectionString.Split(';').Take(2))}'");
            // Build a temporary provider to test the connection
            using (var provider = services.BuildServiceProvider())
            using (var db = provider.GetRequiredService<AIDbContext>())
            {
                if (db.Database.CanConnect())
                {
                    Console.WriteLine("- DB Connected!");
                    db.Database.EnsureCreated(); // This creates tables if they don't exist
                    Console.WriteLine("- Tables created or already exist.");
                }
                else
                {
                    Console.WriteLine("- DB Connection Failed!");
                }
            }
            return services;
        }
    }
}
