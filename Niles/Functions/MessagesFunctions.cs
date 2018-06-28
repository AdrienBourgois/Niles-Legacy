using Discord;
using Discord.WebSocket;
using Niles.Modules.Channels;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal static class MessagesFunctions
    {
        public static async void React(ParsedMessage _message)
        {
            SocketUserMessage user_message = (SocketUserMessage)_message.Message;
            await user_message.AddReactionAsync(new Emoji("😉"));
        }

        public static async void ReplyDm(ParsedMessage _message)
        {
            IDMChannel dm = _message.Message.Author.GetOrCreateDMChannelAsync().Result;
            await dm.SendMessageAsync(_message.ConfigParameter);
        }

        public static async void Annonce(ParsedMessage _message)
        {
            if (_message.Sentence == null)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketTextChannel socket_text_channel in Data.Guild.TextChannels)
            {
                if (socket_text_channel.Name != Data.Configuration["AnnoncesChannelName"]) continue;

                await socket_text_channel.SendMessageAsync(_message.Sentence);
                break;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot || user.Roles.Count == 0) continue;

                IDMChannel dm = user.GetOrCreateDMChannelAsync().Result;
                await dm.SendMessageAsync(_message.Sentence);
            }
        }

        public static async void SendToAllPlayers(ParsedMessage _message)
        {
            if (_message.Sentence == null)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot || user.Roles.Count == 0) continue;

                IDMChannel dm = user.GetOrCreateDMChannelAsync().Result;
                await dm.SendMessageAsync(_message.Sentence);
            }
        }

        public static async void Reply(ParsedMessage _message)
        {
            await _message.Message.Channel.SendMessageAsync(_message.ConfigParameter);
        }

        public static async void DeleteMessage(ParsedMessage _message)
        {
            await _message.Message.DeleteAsync();
        }

        public static async void SendToStaffChannel(ParsedMessage _message)
        {
            foreach (SocketTextChannel socket_text_channel in Data.Guild.TextChannels)
            {
                if (socket_text_channel.Name != Data.Configuration["StaffChannelName"]) continue;

                await socket_text_channel.SendMessageAsync(_message.ConfigParameter);
                break;
            }
        }

        public static async void SendToGeneralChannel(ParsedMessage _message)
        {
            foreach (SocketTextChannel socket_text_channel in Data.Guild.TextChannels)
            {
                if (socket_text_channel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socket_text_channel.SendMessageAsync(_message.ConfigParameter);
                break;
            }
        }

        public static async void SendToGeneralChannelAsBot(ParsedMessage _message)
        {
            if (_message.Sentence == null)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message !");
                return;
            }

            foreach (SocketTextChannel socket_text_channel in Data.Guild.TextChannels)
            {
                if (socket_text_channel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socket_text_channel.SendMessageAsync(_message.Sentence);
                break;
            }
        }

        public static async void SendToStaffMembers(ParsedMessage _message)
        {
            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!Tools.IsAdmin(user)) continue;

                IDMChannel dm_channel = user.GetOrCreateDMChannelAsync().Result;
                await dm_channel.SendMessageAsync(_message.ConfigParameter);
            }
        }

        public static async void SendToConnectedStaffMembers(ParsedMessage _message)
        {
            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!Tools.IsAdmin(user)) continue;
                if (user.Status != UserStatus.Online) continue;

                IDMChannel dm_channel = user.GetOrCreateDMChannelAsync().Result;
                await dm_channel.SendMessageAsync(_message.ConfigParameter);
            }
        }

        public static async void SendToMaster(ParsedMessage _message)
        {
            await Bot.DiscordClient.GetUser(ulong.Parse(Data.Configuration["MasterId"])).GetOrCreateDMChannelAsync()
                .Result.SendMessageAsync("Send from " + _message.Message.Author + " : " + _message.Message.Content);
        }

        public static async void CreateChannel(ParsedMessage _message)
        {
            if (_message.Sentence== null)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais nom de channel !");
                return;
            }

            ModuleManager.GetModule<ChannelManager>().CreateChannel(_message.Message.Author, _message.Sentence);
        }
    }
}
