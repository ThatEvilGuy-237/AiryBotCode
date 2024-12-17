using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.Comands
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        [Summary("Test Comand")]
        public async Task ExecuteAsync([Remainder][Summary("bla bla")] string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
            {
                await ReplyAsync($"Usage: !hello <phrase>");
                return;
            }

            await ReplyAsync(phrase);
        }
    }
}
