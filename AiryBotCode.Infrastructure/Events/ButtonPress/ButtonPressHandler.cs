//using AiryBotCode.Events.SlashCommands.Commands;
//using Discord.WebSocket;

//namespace AiryBotCode.Events.ButtonPress
//{
//    public class ButtonPressHandler : MyEventHandeler
//    {
//        public ButtonPressHandler(IServiceProvider serviceProvider) : base(serviceProvider)
//        {

//        }

//        public async Task HandleButtonInteraction(SocketMessageComponent component)
//        {
//            //var guild = _client.GetGuild(component.GuildId.Value);
//            var buttonValue = component.Data.CustomId;
//            CustomIdEncription button = new CustomIdEncription();
//            button.Decrypt(buttonValue);
//            // somthing not realy needed now
//            switch (button.Command)
//            {
//                case UserLogsCommand.Name:
//                    await UserLogsCommand.HandleButtonPress(component, button);
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}
