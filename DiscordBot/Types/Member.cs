using System;
using System.IO;
using Discord;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Types
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
            StreamReader file = File.OpenText("Save/Members/" + _memberUsername);

            dynamic memberDatas = JObject.Parse(file.ReadToEnd());
            DiscordUser = Data.Guild.GetUser((ulong)memberDatas.Id);
            if (DiscordUser == null) return;

            LastConnection = memberDatas.LastConnection;

            file.Close();
            file.Dispose();

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
            StreamWriter file = File.CreateText("Save/Members/" + DiscordUser.Id + ".json");
            dynamic saveObject = new JObject();

            saveObject.Username = DiscordUser.Username;
            saveObject.Id = DiscordUser.Id;
            saveObject.LastConnection = LastConnection;

            file.Write(saveObject.ToString());
            file.Flush();
            file.Close();
            file.Dispose();
        }

        public bool IsValid { get; }

        public readonly IUser DiscordUser;

        private DateTime LastConnection { get; set; } = DateTime.MinValue;
    }
}
