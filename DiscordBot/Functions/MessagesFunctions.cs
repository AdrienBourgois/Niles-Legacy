using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    internal static class MessagesFunctions
    {
        public static async void React(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            SocketUserMessage userMessage = (SocketUserMessage)_message;
            await userMessage.AddReactionAsync(new Emoji("😉"));
        }

        public static async void ReplyDm(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            IDMChannel dm = _message.Author.GetOrCreateDMChannelAsync().Result;
            await dm.SendMessageAsync(_sentence);
        }

        public static async void Annonce(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            if (_sentence == null)
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["AnnoncesChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_sentence);
                break;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot || user.Roles.Count == 0) continue;

                IDMChannel dm = user.GetOrCreateDMChannelAsync().Result;
                await dm.SendMessageAsync(_sentence);
            }
        }

        public static async void SendToAllPlayers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            if (_sentence == null)
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot || user.Roles.Count == 0) continue;

                IDMChannel dm = user.GetOrCreateDMChannelAsync().Result;
                await dm.SendMessageAsync(_sentence);
            }
        }

        public static async void Reply(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            await _message.Channel.SendMessageAsync(_sentence);
        }

        public static async void DeleteMessage(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            await _message.DeleteAsync();
        }

        public static async void SendToStaffChannel(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["StaffChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_sentence);
                break;
            }
        }

        public static async void SendToGeneralChannel(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_sentence);
                break;
            }
        }

        public static async void SendToGeneralChannelAsBot(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            if (_sentence == null)
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message !");
                return;
            }

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_sentence);
                break;
            }
        }

        public static async void SendToStaffMembers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!Tools.IsAdmin(user)) continue;

                IDMChannel dmChannel = user.GetOrCreateDMChannelAsync().Result;
                await dmChannel.SendMessageAsync(_sentence);
            }
        }

        public static async void SendToConnectedStaffMembers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!Tools.IsAdmin(user)) continue;
                if (user.Status != UserStatus.Online) continue;

                IDMChannel dmChannel = user.GetOrCreateDMChannelAsync().Result;
                await dmChannel.SendMessageAsync(_sentence);
            }
        }

        public static async void SendToMaster(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            await Bot.DiscordClient.GetUser(ulong.Parse(Data.Configuration["MasterId"])).GetOrCreateDMChannelAsync()
                .Result.SendMessageAsync("Send from " + _message.Author + " : " + _message.Content);
        }

        public static async void CreateChannel(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            if (_sentence == null)
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais nom de channel !");
                return;
            }

            ModuleManager.GetModule<ChannelManager>().CreateChannel(_message.Author, _sentence);
        }
    }
}
