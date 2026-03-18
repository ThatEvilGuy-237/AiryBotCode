using AiryBotCode.Application.Services;
using AiryBotCode.Domain.database;
using AiryBotCode.Application.Frontend;
using AiryBotCode.Application.Services.Database.GiveAway;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Application.Features.Giveaway
{
    public class GiveawayCommand : EvilCommand
    {
        private readonly GiveAwayUserService _giveAwayUserService;

        // Replace with your actual channel ID
        private const ulong ScoreboardChannelId = 1182267222152982535; // IMPORTANT: REPLACE THIS

        private static readonly Dictionary<ulong, ulong> ScoreboardMessageIds = new Dictionary<ulong, ulong>();
        private static readonly Dictionary<ulong, (ulong messageId, ulong channelId)> GiveawayMessageIds = new Dictionary<ulong, (ulong, ulong)>();

        public const string RegisterAction = "start-event";
        public const string GetAllUsersAction = "get-all-users";
        public const string GetRandomAction = "get-random";
        public const string EndEventAction = "end-event";
        public const string ClearUsersAction = "clear-users";
        public const string ConfirmClearUsersAction = "confirm-clear-users";

        public GiveawayCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "start-event";
            _giveAwayUserService = serviceProvider.GetRequiredService<GiveAwayUserService>();
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Starts a new giveaway event.");
        }

        public async Task HandleStartEventCommand(SocketSlashCommand command)
        {
            var guildId = command.GuildId.Value;
            var scoreboardChannel = _client.GetChannel(ScoreboardChannelId) as SocketTextChannel;
            if (scoreboardChannel == null)
            {
                await command.RespondAsync("Error: Scoreboard channel not found. Please configure the bot.", ephemeral: true);
                return;
            }

            // Create and send initial scoreboard embed
            var allUsers = await _giveAwayUserService.GetAllUsers();
            var initialScoreboardEmbed = GiveawayFrontend.CreateScoreboardEmbed(allUsers.Count, "Accepting Registrations");
            var buttonEncryptorAllUsers = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = GetAllUsersAction
            };
            var buttonEncryptorGetRandom = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = GetRandomAction
            };
            var buttonEncryptorEndEvent = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = EndEventAction
            };
            var buttonEncryptorClearUsers = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = ClearUsersAction
            };

            var scoreboardComponents = new ComponentBuilder()
                .WithButton("Get All Users", buttonEncryptorAllUsers.Encript(), ButtonStyle.Primary)
                .WithButton("Get Random", buttonEncryptorGetRandom.Encript(), ButtonStyle.Success)
                .WithButton("End Event", buttonEncryptorEndEvent.Encript(), ButtonStyle.Danger)
                .WithButton("Clear Users", buttonEncryptorClearUsers.Encript(), ButtonStyle.Danger)
                .Build();

            if (ScoreboardMessageIds.TryGetValue(guildId, out ulong messageId))
            {
                var existingMessage = await scoreboardChannel.GetMessageAsync(messageId) as IUserMessage;
                if (existingMessage != null)
                {
                    await existingMessage.ModifyAsync(props =>
                    {
                        props.Embed = initialScoreboardEmbed;
                        props.Components = scoreboardComponents;
                    });
                }
                else
                {
                    var scoreboardMessage = await scoreboardChannel.SendMessageAsync(embed: initialScoreboardEmbed, components: scoreboardComponents);
                    ScoreboardMessageIds[guildId] = scoreboardMessage.Id;
                }
            }
            else
            {
                var scoreboardMessage = await scoreboardChannel.SendMessageAsync(embed: initialScoreboardEmbed, components: scoreboardComponents);
                ScoreboardMessageIds[guildId] = scoreboardMessage.Id;
            }

            // Create the main giveaway embed and button
            var giveawayEmbed = GiveawayFrontend.CreateGiveawayEmbed();
            var buttonEncryptor = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = RegisterAction
            };

            var components = new ComponentBuilder()
                .WithButton("Click to Register!", buttonEncryptor.Encript(), ButtonStyle.Success)
                .Build();

            await command.RespondAsync(embed: giveawayEmbed, components: components);
            var giveawayMessage = await command.GetOriginalResponseAsync();
            GiveawayMessageIds[guildId] = (giveawayMessage.Id, command.ChannelId.Value);
        }

        public async Task HandleButtonInteraction(SocketMessageComponent component, ButtonEncriptionService buttonData)
        {
            if (buttonData.Action == RegisterAction)
            {
                await HandleRegisterButton(component);
            }
            else if (buttonData.Action == GetAllUsersAction)
            {
                await HandleGetAllUsersButton(component);
            }
            else if (buttonData.Action == GetRandomAction)
            {
                await HandleGetRandomButton(component);
            }
            else if (buttonData.Action == EndEventAction)
            {
                await HandleEndEventButton(component);
            }
            else if (buttonData.Action == ClearUsersAction)
            {
                await HandleClearUsersButton(component);
            }
            else if (buttonData.Action == ConfirmClearUsersAction)
            {
                await HandleConfirmClearUsersButton(component);
            }
            else if (component.Data.CustomId == "cancel-clear")
            {
                await component.Message.DeleteAsync();
            }
        }

        private async Task HandleClearUsersButton(SocketMessageComponent component)
        {
            if (component.User.Id != 405431299323461634)
            {
                await component.RespondAsync("You do not have permission to use this button.", ephemeral: true);
                return;
            }

            var buttonEncryptor = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = ConfirmClearUsersAction
            };

            var confirmationComponents = new ComponentBuilder()
                .WithButton("Yes, I'm sure", buttonEncryptor.Encript(), ButtonStyle.Danger)
                .WithButton("Cancel", "cancel-clear", ButtonStyle.Secondary)
                .Build();

            await component.RespondAsync("Are you sure you want to clear all users? This action cannot be undone.", components: confirmationComponents, ephemeral: true);
        }

        private async Task HandleConfirmClearUsersButton(SocketMessageComponent component)
        {
            if (component.User.Id != 405431299323461634)
            {
                await component.RespondAsync("You do not have permission to use this button.", ephemeral: true);
                return;
            }

            await _giveAwayUserService.DeleteAllUsersAsync();

            var guildId = component.GuildId.Value;
            if (ScoreboardMessageIds.TryGetValue(guildId, out var scoreboardMessageId))
            {
                var scoreboardChannel = _client.GetChannel(ScoreboardChannelId) as SocketTextChannel;
                var scoreboardMessage = await scoreboardChannel.GetMessageAsync(scoreboardMessageId) as IUserMessage;
                if (scoreboardMessage != null)
                {
                    var updatedEmbed = GiveawayFrontend.CreateScoreboardEmbed(0, "Accepting Registrations");
                    await scoreboardMessage.ModifyAsync(props => props.Embed = updatedEmbed);
                }
            }
            
            await component.UpdateAsync(props => {
                props.Content = "All users have been cleared from the giveaway.";
                props.Components = new ComponentBuilder().Build();
            });
        }


        private async Task HandleGetRandomButton(SocketMessageComponent component)
        {
            var customId = new ButtonEncriptionService
            {
                CommandName = Name,
                Action = GetRandomAction,
                MessagesId = new List<ulong> { component.Message.Id },
                ChannelsId = new List<ulong> { component.Channel.Id },
            };
            var modal = GiveawayFrontend.CreateGetRandomForm(customId.Encript());
            await component.RespondWithModalAsync(modal.Build());
        }

        private async Task HandleEndEventButton(SocketMessageComponent component)
        {
            var guildId = component.GuildId.Value;
            if (GiveawayMessageIds.TryGetValue(guildId, out var giveawayInfo))
            {
                var giveawayChannel = _client.GetChannel(giveawayInfo.channelId) as SocketTextChannel;
                if (giveawayChannel != null)
                {
                    var giveawayMessage = await giveawayChannel.GetMessageAsync(giveawayInfo.messageId) as IUserMessage;
                    if (giveawayMessage != null)
                    {
                        var updatedComponents = new ComponentBuilder()
                            .WithButton("Registration Closed", "event-ended", ButtonStyle.Secondary, disabled: true)
                            .Build();

                        await giveawayMessage.ModifyAsync(props =>
                        {
                            props.Components = updatedComponents;
                        });

                        await component.RespondAsync("Giveaway registration has been closed.", ephemeral: true);
                        //GiveawayMessageIds.Remove(guildId); // Clean up
                    }
                }
            }
            if (ScoreboardMessageIds.TryGetValue(guildId, out var scoreboardMessageId))
            {
                var scoreboardChannel = _client.GetChannel(ScoreboardChannelId) as SocketTextChannel;
                var scoreboardMessage = await scoreboardChannel.GetMessageAsync(scoreboardMessageId) as IUserMessage;
                if (scoreboardMessage != null)
                {
                    var allUsers = await _giveAwayUserService.GetAllUsers();
                    var updatedEmbed = GiveawayFrontend.CreateScoreboardEmbed(allUsers.Count, "Ended");
                    await scoreboardMessage.ModifyAsync(props => props.Embed = updatedEmbed);
                }
            }
        }

        public async Task HandleGetRandomForm(SocketModal modal)
        {
            var components = modal.Data.Components.ToList();
            var numUsersComponent = components.FirstOrDefault(c => c.CustomId == "num_users");

            if (int.TryParse(numUsersComponent?.Value, out int numUsersToSelect) && numUsersToSelect > 0)
            {
                var winners = await _giveAwayUserService.GetRandomUsers(numUsersToSelect);

                if (winners.Count == 0)
                {
                    await modal.RespondAsync("No users registered for the giveaway yet.", ephemeral: true);
                    return;
                }
                if (winners.Count < numUsersToSelect)
                {
                    await modal.RespondAsync($"Not enough users registered to select {numUsersToSelect} winners. Only {winners.Count} users were selected.", ephemeral: true);
                    return;
                }

                var winnerMentions = winners.Select(u => $"<@{u.Id}>").ToList();
                var winnerMessage = $"🎉 Congratulations to the winners: {string.Join(", ", winnerMentions)}!";

                await modal.Channel.SendMessageAsync(winnerMessage);
                await modal.RespondAsync("Winners have been announced!", ephemeral: true);
            }
            else
            {
                await modal.RespondAsync("Invalid number provided.", ephemeral: true);
            }
        }


        private async Task HandleRegisterButton(SocketMessageComponent component)
        {
            var user = component.User;
            var guildId = component.GuildId.Value;

            if (await _giveAwayUserService.IsUserRegistered(user.Id))
            {
                await component.RespondAsync("You are already registered for the giveaway!", ephemeral: true);
                return;
            }

            var giveAwayUser = new GiveAwayUser { Id = user.Id, UserName = user.Username };
            await _giveAwayUserService.CreateUser(giveAwayUser);

            await component.RespondAsync("You have been successfully registered for the giveaway!", ephemeral: true);

            // Update scoreboard
            var allUsers = await _giveAwayUserService.GetAllUsers();
            var scoreboardChannel = _client.GetChannel(ScoreboardChannelId) as SocketTextChannel;
            if (scoreboardChannel != null && ScoreboardMessageIds.TryGetValue(guildId, out var messageId))
            {
                var scoreboardMessage = await scoreboardChannel.GetMessageAsync(messageId) as IUserMessage;
                if (scoreboardMessage != null)
                {
                    var updatedEmbed = GiveawayFrontend.CreateScoreboardEmbed(allUsers.Count, "Accepting Registrations");
                     var buttonEncryptor = new ButtonEncriptionService
                    {
                        CommandName = Name,
                        Action = GetAllUsersAction
                    };
                    var buttonEncryptorGetRandom = new ButtonEncriptionService
                    {
                        CommandName = Name,
                        Action = GetRandomAction
                    };
                    var buttonEncryptorEndEvent = new ButtonEncriptionService
                    {
                        CommandName = Name,
                        Action = EndEventAction
                    };
                    var buttonEncryptorClearUsers = new ButtonEncriptionService
                    {
                        CommandName = Name,
                        Action = ClearUsersAction
                    };
                    var components = new ComponentBuilder()
                        .WithButton("Get All Users", buttonEncryptor.Encript(), ButtonStyle.Primary)
                        .WithButton("Get Random", buttonEncryptorGetRandom.Encript(), ButtonStyle.Success)
                        .WithButton("End Event", buttonEncryptorEndEvent.Encript(), ButtonStyle.Danger)
                        .WithButton("Clear Users", buttonEncryptorClearUsers.Encript(), ButtonStyle.Danger)
                        .Build();
                    await scoreboardMessage.ModifyAsync(props => {
                        props.Embed = updatedEmbed;
                        props.Components = components;
                    });
                }
            }
        }
        
        private async Task HandleGetAllUsersButton(SocketMessageComponent component)
        {
            var allUsers = await _giveAwayUserService.GetAllUsers();

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("# All users");

            foreach (var user in allUsers)
            {
                var userLine = $"- <@{user.Id}>\n";
                if (messageBuilder.Length + userLine.Length > 1800)
                {
                    await component.Channel.SendMessageAsync(messageBuilder.ToString());
                    messageBuilder.Clear();
                }
                messageBuilder.Append(userLine);
            }

            if (messageBuilder.Length > 0)
            {
                await component.Channel.SendMessageAsync(messageBuilder.ToString());
            }

            await component.RespondAsync("User list generated!", ephemeral: true);
        }
    }
}