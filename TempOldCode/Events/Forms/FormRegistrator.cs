using AiryBotCode.Events.ButtonPress;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.Forms
{
    public static class FormRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register as Singleton
            services.AddSingleton<FormHandler>();

            return services;
        }
    }
}
