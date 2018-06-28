using System;
using System.IO;
using Discord;
using Newtonsoft.Json.Linq;

namespace Niles.Types
{
    internal class Member
    {
        public Member(IUser _discordUser)
        {
            DiscordUser = _discordUser;
            IsValid = true;
        }

        public Member(ulong _id)
        {
            DiscordUser = Data.Guild.GetUser(_id);

            if(DiscordUser != null)
                IsValid = true;
        }

        public Member(string _memberUsername)
        {
            string file_content = File.ReadAllText("Save/Members/" + _memberUsername);

            dynamic member_datas = JObject.Parse(file_content);
            DiscordUser = Data.Guild.GetUser((ulong)member_datas.Id);
            if (DiscordUser == null) return;

            LastConnection = member_datas.LastConnection;

            IsValid = true;
        }

        public void OnConnected()
        {
            LastConnection = Tools.TimeNow();
        }

        public Embed GetEmbedInfos()
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(DiscordUser.Username)
                .WithDescription("Fiche de membre")
                .WithColor(new Color(0x680F90))
                .WithThumbnailUrl(DiscordUser.GetAvatarUrl())
                .AddField("Dernière connexion Vocale :", LastConnection != DateTime.MinValue ? LastConnection.ToString(@"dd\/MM\/yyyy HH:mm") : "Jamais");

            return builder.Build();
        }

        public void Save()
        {
            dynamic save_object = new JObject();

            save_object.Username = DiscordUser.Username;
            save_object.Id = DiscordUser.Id;
            save_object.LastConnection = LastConnection;

            File.WriteAllText("Save/Members/" + DiscordUser.Id + ".json", save_object.ToString());
        }

        public bool IsValid { get; }

        public readonly IUser DiscordUser;

        private DateTime LastConnection { get; set; } = DateTime.MinValue;
    }
}
