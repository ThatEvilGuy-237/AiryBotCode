using AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.SendMessage
{
    public static class MessageSendRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register as Singleton
            services.AddSingleton<MessageSendHandler>();
            services.AddSingleton<TalkWithAiryManager>();

            return services;
        }
    }
}
