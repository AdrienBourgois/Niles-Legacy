using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Niles.Modules.Members;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal static class MemberFunctions
    {
        public static async void GetMemberInformations(ParsedMessage _message)
        {
            SocketMessage message = _message.Message;
            List<string> parameters = _message.Parameters;

            if (message.MentionedUsers.Count == 0 && (parameters == null || parameters.Count == 0 || !Tools.IsMemberIdValid(parameters[0])))
            {
                await message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Pas de membre défini");
                return;
            }

            ulong id = message.MentionedUsers.Count != 0
                ? message.MentionedUsers.First().Id
                : ulong.Parse(parameters?[0]);

            Member member = ModuleManager.GetModule<MemberManager>().GetMember(id);
            if (member != null)
            {
                Embed member_embed = member.GetEmbedInfos();
                await message.Channel.SendMessageAsync("", embed: member_embed).ConfigureAwait(false);
            }
            else
            {
                await message.Channel.SendMessageAsync(":negative_squared_cross_mark: Erreur : Membre inconnu");
            }
        }

        public static void SaveMembers(ParsedMessage _message)
        {
            ModuleManager.GetModule<MemberManager>().SaveMembers();
        }

        public static void LoadMembers(ParsedMessage _message)
        {
            ModuleManager.GetModule<MemberManager>().LoadMembers();
        }
    }
}
