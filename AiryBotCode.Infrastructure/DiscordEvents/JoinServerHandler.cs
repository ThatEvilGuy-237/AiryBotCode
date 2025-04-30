//using Discord.WebSocket;
//using Discord;
//using Discord.Commands;
//using Microsoft.Extensions.DependencyInjection;

//namespace AiryBotCode.Events.JoinServer
//{
//    public class JoinServerHandler : MyEventHandeler
//    {
//        public JoinServerHandler(IServiceProvider serviceProvider) 
//            : base(serviceProvider)
//        {

//        }

//        public async Task OnJoinedGuild(SocketGuild guild)
//        {
//            Console.WriteLine($"Joined a new guild: {guild.Name} (ID: {guild.Id})");

//            if (guild.SystemChannel != null)
//            {
//                await guild.SystemChannel.SendMessageAsync($"Hello {guild.Name}! Thanks for inviting me!");
//            }
//            else
//            {
//                Console.WriteLine("No system channel available in the guild.");
//            }
//        }
//        public async Task OnUserJoined(SocketGuildUser user)
//        {
//            // Log user details
//            Console.WriteLine($"User {user.Username}#{user.Discriminator} joined guild {user.Guild.Name}.");

//            // Welcome message
//            var welcomeChannel = user.Guild.DefaultChannel as IMessageChannel; // Default channel
//            if (welcomeChannel != null)
//            {
//                await welcomeChannel.SendMessageAsync($"Welcome {user.Mention} to {user.Guild.Name}! 🎉");
//            }
//            else
//            {
//                Console.WriteLine("No default channel available to send a welcome message.");
//            }
//        }
//    }
//}
