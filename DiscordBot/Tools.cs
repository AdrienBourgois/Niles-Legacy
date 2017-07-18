using System.Linq;
using Discord.WebSocket;

namespace DiscordBot
{
    internal static class Tools
    {
        public static bool IsFromAdmin(SocketMessage _message)
        {
            ulong[] adminsList = { 156607889958502401 };
            return adminsList.Contains(_message.Author.Id);
        }
    }
}
