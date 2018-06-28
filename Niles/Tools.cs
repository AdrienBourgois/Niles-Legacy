using System;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;

namespace Niles
{
    internal static class Tools
    {
        private static readonly string[] rolesStaffNames = { "Fondateur", "Administrateur", "Modérateur", "Leader" };

        public static bool IsFromAdmin(SocketMessage _message)
        {
            SocketGuildUser user = Data.Guild.GetUser(_message.Author.Id);
            return user.Id == ulong.Parse(Data.Configuration["MasterId"]) || user.Roles.Any(_role => rolesStaffNames.Contains(_role.Name)) || user.GuildPermissions.Has(GuildPermission.Administrator);
        }

        public static bool IsAdmin(SocketGuildUser _user)
        {
            return _user.Roles.Any(_role => rolesStaffNames.Contains(_role.Name));
        }

        public static bool IsMemberIdValid(string _string)
        {
            Regex regex = new Regex("^[0-9]{18}$");
            return regex.IsMatch(_string);
        }

        public static DateTime TimeNow()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Romance Standard Time");
        }
    }
}
