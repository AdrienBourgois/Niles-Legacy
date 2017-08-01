using System.Linq;
using Discord.WebSocket;

namespace DiscordBot
{
    internal static class Tools
    {
        private static readonly string[] RolesStaffNames = { "Fondateur", "Administrateur", "Modérateur", "Leader" };

        public static bool IsFromAdmin(SocketMessage _message)
        {
            SocketGuildUser user = Data.Guild.GetUser(_message.Author.Id);
            return user.Roles.Any(_role => RolesStaffNames.Contains(_role.Name));
        }

        public static bool IsAdmin(SocketGuildUser _user)
        {
            return _user.Roles.Any(_role => RolesStaffNames.Contains(_role.Name));
        }
    }
}
