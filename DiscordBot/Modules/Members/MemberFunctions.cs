using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Types;

namespace DiscordBot.Functions
{
    internal static class MemberFunctions
    {
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

            Member member = ModuleManager.GetModule<MemberManager>().GetMember(id);
            if (member != null)
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
            ModuleManager.GetModule<MemberManager>().SaveMembers();
        }

        public static void LoadMembers(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            ModuleManager.GetModule<MemberManager>().LoadMembers();
        }
    }
}
