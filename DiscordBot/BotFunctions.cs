using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordBot.Managers;
using DiscordBot.Types;

namespace DiscordBot
{
    internal static class BotFunctions
    {
        public static void StopBot(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            Bot.AskToStop();
        }

        public static async void Sleep(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            await _message.Channel.SendMessageAsync("A tout à l'heure ! :wave:");
            Bot.Sleep();
        }

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

        public static void AddDifferedMessage(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            DifferedMessagesManager.AddDifferedMessage(_message, 20);
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

            ModuleManager.GetModule<ChannelManager>().CreateChannel((SocketGuildUser) _message.Author, _sentence);
        }

        public static async void SendHelpList(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
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

        public static async void SendAdminHelpList(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> commandPair in ProcessMessage.CommandList.Commands)
            {
                Command command = commandPair.Value;
                if (!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "```\nListe des commandes administateurs : \n```\n";

            foreach (KeyValuePair<string, Command> commandPair in ProcessMessage.CommandList.Commands)
            {
                Command command = commandPair.Value;
                if (command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }

            answer += "\n```";

            await _message.Channel.SendMessageAsync(answer);
        }

        public static async void GetFunctionsNames(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            string answer = "Liste des méthodes : \n```\n";
            answer += typeof(BotFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public).Aggregate("", (_current, _method) => _current + (_method.Name + '\n')) + "\n```";
            await _message.Channel.SendMessageAsync(answer);
        }

        public static void ReloadConfig(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            ProcessMessage.CommandList.PrepareCommands();
        }

        public static async void GetMemberInformations(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            if (_message.MentionedUsers.Count == 0 && (_parameters == null || _parameters.Count == 0 || !Tools.IsMemberIdValid(_parameters[0])))
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas de membre défini");
                return;
            }

            ulong id = _message.MentionedUsers.Count != 0
                ? _message.MentionedUsers.First().Id
                : ulong.Parse(_parameters?[0]);

            Member member = ModuleManager.GetModule<MembersManagers>().GetMember(id);
            if(member != null)
            {
                Embed memberEmbed = member.GetEmbedInfos();
                await _message.Channel.SendMessageAsync("", embed: memberEmbed).ConfigureAwait(false);
            }
            else
            {
                await _message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Membre inconnu");
            }
        }

        public static void SaveMembers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            ModuleManager.GetModule<MembersManagers>().SaveMembers();
        }

        public static void LoadMembers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            ModuleManager.GetModule<MembersManagers>().LoadMembers();
        }

        public static async void SendToLog(SocketMessage _message, string _text, InputCommand _inputs = null)
        {
            if (_text == null)
                return;

            string replace = _text;
            if(_text.Contains("{USER}"))
                replace = replace.Replace("{USER}", _message.Author.Username);
            if(_inputs != null)
                if (_text.Contains("{PARAMETER}"))
                    replace = replace.Replace("{PARAMETER}", _inputs.Parameters.Aggregate((_i, _j) => _i + ", " + _j));

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["LogsChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(replace);
                break;
            }
        }
    }
}
