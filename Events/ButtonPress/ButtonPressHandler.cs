using Discord.WebSocket;

namespace AiryBotCode.Events.ButtonPress
{
    public class ButtonPressHandler
    {
        public static async Task HandleButtonInteraction(SocketMessageComponent component, DiscordSocketClient client)
        {
            var guild = client.GetGuild(component.GuildId.Value);
            //await TimeoutFrontend.HandleButtonInteraction(component, guild);
        }
    }
}
