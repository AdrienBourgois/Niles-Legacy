using System.Linq;
using Discord.WebSocket;

namespace DiscordBot
{
    internal static class Tools
    {
        public static bool IsFromAdmin(SocketMessage _message)
        {
            string[] rolesStaffNames = {"Fondateur", "Administrateur", "Modérateur", "Leader"};
            SocketGuildUser user = Data.Guild.GetUser(_message.Author.Id);
            return user.Roles.Any(_role => rolesStaffNames.Contains(_role.Name));
        }

        public static bool IsAdmin(SocketGuildUser _user)
        {
            string[] rolesStaffNames = { "Fondateur", "Administrateur", "Modérateur", "Leader" };
            return _user.Roles.Any(_role => rolesStaffNames.Contains(_role.Name));
        }
    }
}
