﻿using AiryBotCode.Application.Services.Loging;
using AiryBotCode.Application.Services.User;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class ContactUserCommand : EvilCommand
    {
        protected UserService _userService;
        public ContactUserCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "contact";
            _userService = serviceProvider.GetRequiredService<UserService>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Create channel to contact user")
                .AddOption("target", ApplicationCommandOptionType.User, "who do you want to contact?", isRequired: true);
        }

        public async Task<RestTextChannel> ContractUserCommand(SocketSlashCommand command)
        {
            //if (!await _userService.UserIsAdmin(command))
            //{
            //    await command.RespondAsync("You are not allowed to use this command.", ephemeral: true);
            //    return null;
            //}

            var targetUser = (SocketGuildUser)command.Data.Options.First(x => x.Name == "target").Value;
            var guild = (command.Channel as SocketGuildChannel)?.Guild;
            if (guild == null)
            {
                await command.RespondAsync("This command must be used in a server.", ephemeral: true);
                return null;
            }

            ulong categoryId = 1234577123541258280;
            var category = guild.GetCategoryChannel(categoryId);
            if (category == null)
            {
                await command.RespondAsync("Private category not found.", ephemeral: true);
                return null;
            }

            // Create the channel
            var channel = await guild.CreateTextChannelAsync($"contact-{targetUser.Username}", props =>
            {
                props.CategoryId = category.Id;
            });

            // Add target user to it
            await channel.AddPermissionOverwriteAsync(targetUser, new OverwritePermissions(
                viewChannel: PermValue.Allow,
                sendMessages: PermValue.Allow
            ));

            await command.RespondAsync($"Private contact channel created: {channel.Mention}", ephemeral: true);

            return channel;
        }
        public async Task SendMessage(RestTextChannel channel)
        {
           
            var embed = new EmbedBuilder()
                .WithTitle("Private Contact")
                .WithTitle("✨ Private Channel ✨")
                    .WithDescription("Hey there! A moderator opened this channel so we can chat with you in private. 💬")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .Build();

            await channel.SendMessageAsync(embed: embed);
        }

    }
}
