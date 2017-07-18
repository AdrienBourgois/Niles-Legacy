using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot
{
    internal static class BotFunctions
    {
        public static void StopBot(SocketMessage _message, string _text)
        {
            Bot.AskToStop();
        }

        public static async void React(SocketMessage _message, string _text)
        {
            SocketUserMessage userMessage = (SocketUserMessage)_message;
            await userMessage.AddReactionAsync(new Emoji("😉"));
        }

        public static async void ReplyDm(SocketMessage _message, string _text)
        {
            RestDMChannel dm = _message.Author.CreateDMChannelAsync().Result;
            await dm.SendMessageAsync(_text);
        }

        public static async void Annonce(SocketMessage _message, string _text)
        {
            string messageString = _message.Content.Substring(_message.Content.IndexOf(' ') + 1);

            if (messageString.StartsWith("!"))
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["AnnoncesChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(messageString);
                break;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot) continue;

                Task<RestDMChannel> dmTask = user.CreateDMChannelAsync();
                dmTask.Wait();
                await dmTask.Result.SendMessageAsync(messageString);
            }
        }

        public static async void SendToAllPlayers(SocketMessage _message, string _text)
        {
            string messageString = _message.Content.Substring(_message.Content.IndexOf(' ') + 1);

            if (messageString.StartsWith("!"))
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message d'annonce !");
                return;
            }

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (user.IsBot) continue;

                Task<RestDMChannel> dmTask = user.CreateDMChannelAsync();
                dmTask.Wait();
                await dmTask.Result.SendMessageAsync(messageString);
            }
        }

        public static void AddDifferedMessage(SocketMessage _message, string _text)
        {
            DifferedMessagesManager.AddDifferedMessage(_message, 20);
        }

        public static async void Reply(SocketMessage _message, string _text)
        {
            await _message.Channel.SendMessageAsync(_text);
        }

        public static async void DeleteMessage(SocketMessage _message, string _text)
        {
            await _message.DeleteAsync();
        }

        public static async void SendToStaffChannel(SocketMessage _message, string _text)
        {
            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["StaffChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_text);
                break;
            }
        }

        public static async void SendToGeneralChannel(SocketMessage _message, string _text)
        {
            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(_text);
                break;
            }
        }

        public static async void SendToGeneralChannelAsBot(SocketMessage _message, string _text)
        {
            string messageString = _message.Content.Substring(_message.Content.IndexOf(' ') + 1);

            if (messageString.StartsWith("!"))
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais message !");
                return;
            }

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["GeneralChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(messageString);
                break;
            }
        }

        public static async void SendToStaffMembers(SocketMessage _message, string _text)
        {
            SocketRole staffRole = null;

            foreach (SocketRole role in Data.Guild.Roles)
            {
                if (role.Name == Data.Configuration["StaffRoleName"])
                    staffRole = role;
            }

            if (staffRole == null)
                return;

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!user.Roles.Contains(staffRole)) continue;

                RestDMChannel dmChannel = user.CreateDMChannelAsync().Result;
                await dmChannel.SendMessageAsync(_text);
            }
        }

        public static async void SendToConnectedStaffMembers(SocketMessage _message, string _text)
        {
            SocketRole staffRole = null;

            foreach (SocketRole role in Data.Guild.Roles)
            {
                if (role.Name == Data.Configuration["StaffRoleName"])
                    staffRole = role;
            }

            if (staffRole == null)
                return;

            foreach (SocketGuildUser user in Data.Guild.Users)
            {
                if (!user.Roles.Contains(staffRole)) continue;
                if (user.Status != UserStatus.Online) continue;

                RestDMChannel dmChannel = user.CreateDMChannelAsync().Result;
                await dmChannel.SendMessageAsync(_text);
            }
        }

        public static async void SendToMaster(SocketMessage _message, string _text)
        {
            await Bot.DiscordClient.GetUser(ulong.Parse(Data.Configuration["MasterId"])).CreateDMChannelAsync()
                .Result.SendMessageAsync("Send from " + _message.Author + " : " + _message.Content);
        }

        public static async void CreateChannel(SocketMessage _message, string _text)
        {
            string name = _message.Content.Substring(_message.Content.IndexOf(' ') + 1);

            if (name.StartsWith("!"))
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas ou mauvais nom de channel !");
                return;
            }

            RealTimeUpdate.ChannelManager.CreateChannel((SocketGuildUser) _message.Author, name);
        }

        public static async void SendHelpList(SocketMessage _message, string _text)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> commandPair in ProcessMessage.CommandList.Commands)
            {
                Command command = commandPair.Value;
                if(!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "\n```";

            await _message.Channel.SendMessageAsync(answer);
        }

        public static void ReloadConfig(SocketMessage _message, string _text)
        {
            ProcessMessage.CommandList.PrepareCommands();
        }

        public static async void SendToLog(SocketMessage _message, string _text, string _parameter)
        {
            if (_text == null)
                return;

            string replace = _text;
            if(_text.Contains("{USER}"))
                replace = replace.Replace("{USER}", _message.Author.Username);
            if (_text.Contains("{PARAMETER}"))
                replace = replace.Replace("{PARAMETER}", _message.Content.Substring(_message.Content.IndexOf(' ') + 1));

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["LogsChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(replace);
                break;
            }
        }
    }
}
