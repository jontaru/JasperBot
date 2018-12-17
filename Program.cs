﻿using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using JasperBot.Core.Commands;
using JasperBot.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace JasperBot
{
    class Program
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        public static RestUserMessage Listing { get; set; }
        public static List<CraftingRequest> Requests = new List<CraftingRequest>();

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug                
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug                
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
                        
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            Client.ReactionAdded += onReactionAdded;
<<<<<<< HEAD
            string Token = "";            
=======
            string Token = "NTIzODkyMjkzNTE4ODg0ODY0.DvgImw.3Ozvt3SqludiGZbPKbu1VAXWQME";            
>>>>>>> 6a33eba18d577a33ef556c4edd6368118fd58c1a
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task onReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var request = Requests.Find(r => r.messageId == reaction.MessageId);
            if (request != null)
            {
                if (reaction.Emote.Name == "⏲")
                {
                    request.assignedCrafter = reaction.User.Value.Username;
                    request.status = OrderStatus.Assigned;                    
                } else if (reaction.Emote.Name == "✅")
                {
                    if (request.assignedCrafter == "Unassigned")
                        request.assignedCrafter = reaction.User.Value.Username;
                    request.status = OrderStatus.InProgress;                    
                } else if (reaction.Emote.Name == "📦")
                {
                    if (request.assignedCrafter == "Unassigned")
                        request.assignedCrafter = reaction.User.Value.Username;
                    request.status = OrderStatus.Ready;                    
                } else if (reaction.Emote.Name == "🛄")
                {
                    if (request.assignedCrafter == "Unassigned")
                        request.assignedCrafter = reaction.User.Value.Username;
                    request.status = OrderStatus.Completed;                    
                                        
                    var archives = Client.GetChannel(524011068285255696) as SocketTextChannel;
                    await archives.SendMessageAsync($"[{DateTime.Now.ToShortDateString()}] Requester: {request.Requester} | Crafter: {request.assignedCrafter} | {request.quantity}x {request.itemName}");
                    var message = await channel.GetMessageAsync(reaction.MessageId);
                    await message.DeleteAsync();
                }

                Utilities.UpdateListing(channel);
            }

            //return Task.CompletedTask;
        }

        private async Task Client_Log(LogMessage message)
        {
            Console.WriteLine($"{DateTime.Now} - [{message.Source}] - {message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Crafting Request Handling","https://twitch.tv/sadistikgaming", StreamType.NotStreaming);
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;            
            if (!Context.IsPrivate && Context.Channel.Name != "crafting-requests") return;

            int ArgPos = 0;
            if (!(Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} [{MessageParam.Source}] Something went wrong with execvuting a command: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }
        }
    }
}
