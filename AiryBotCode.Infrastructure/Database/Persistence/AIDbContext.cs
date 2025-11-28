using AiryBotCode.Application.Interfaces;
using AiryBotCode.Domain.database;
using AiryBotCode.Infrastructure.Database.Persistence.Config;
using AiryBotCode.Infrastructure.Database.Seeders;
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
        public DbSet<BotSetting> BotSettings { get; set; }
        public DbSet<GiveAwayUser> GiveAwayUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ChatUserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConversationConfiguration());
            modelBuilder.ApplyConfiguration(new BotSettingConfiguration());
            modelBuilder.ApplyConfiguration(new GiveAwayUserConfiguration());
        }

        public static  IServiceCollection registerDbContext(IServiceCollection services)
        {
            // Construct the connection string directly from IConfiguration
            IConfigurationReader _config = services.BuildServiceProvider().GetRequiredService<IConfigurationReader>();
            string connectionString = _config.GetDatabaseConnectionString();

            if (string.IsNullOrWhiteSpace(_config.GetDatabaseHost()))
                throw new InvalidOperationException("Database configuration 'Host' is missing from appsettings.json.");
            services.AddDbContext<AIDbContext>(options =>
                options.UseNpgsql(connectionString));
            Console.WriteLine($"[DATABASE] Connecting to database... '{string.Join(";", connectionString.Split(';').Take(2))}'");

            // Test database connection.
            TestDbConnection(services);
            SeedData(services.BuildServiceProvider()).Wait();
            return services;
        }

        private static async Task SeedData(IServiceProvider serviceProvider)
        {
            try
            {
                await BotSettingsSeeder.Seed(serviceProvider);
            }
            catch (Exception ex) { }
        }

        private static void TestDbConnection(IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AIDbContext>();
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine($"- DB Connection Failed with exception: {ex.Message}");
                }
            }
        }
    }
}
