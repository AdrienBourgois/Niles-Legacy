using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Functions;
using DiscordBot.Interface;
using DiscordBot.Types;

namespace DiscordBot.Modules
{
    internal class MemberManager : IEventsModule
    {
        private readonly Dictionary<ulong, Member> members = new Dictionary<ulong, Member>();

        private readonly Timer timer;

        public MemberManager()
        {
            Bot.DiscordClient.Ready += delegate { LoadMembers(); return null; };
            timer = new Timer(SaveMembers);
        }

        public Member GetMember(ulong _id)
        {
            if (members.ContainsKey(_id))
                return members[_id];

            Member member = new Member(_id);

            if (!member.IsValid) return null;

            members.Add(member.DiscordUser.Id, member);
            return member;
        }

        private Task OnUserVoiceStateUpdated(SocketUser _socketUser, SocketVoiceState _previouState, SocketVoiceState _newState)
        {
            if (_newState.VoiceChannel == null || _newState.VoiceChannel.Guild != Data.Guild) return null;
            if (_newState.VoiceChannel.Name.Contains("Absent")) return null;

            GetMember(_socketUser.Id).OnConnected();

            SocketGuildUser user = Data.Guild.GetUser(_socketUser.Id);

            if (user.Roles.Count != 1 || !user.Roles.First().IsEveryone) return null;
            if (_previouState.VoiceChannel == _newState.VoiceChannel) return null;
            if (_newState.VoiceChannel == null) return null;
            if (_newState.VoiceChannel.Name.Contains(Data.Configuration["AccueilVoiceChannelName"]) && _newState.VoiceChannel.Users.Count == 1)
                MessagesFunctions.SendToConnectedStaffMembers(null, Data.GetRandomSentence(Data.SentenceType.StaffNewbieOnVoiceChannel, "USER", user.Username));

            return null;
        }

        public void LoadMembers()
        {
            members.Clear();
            if (!Directory.Exists("Save/Members")) return;

            foreach (FileInfo fileInfo in new DirectoryInfo("Save/Members").GetFiles())
            {
                Member member = new Member(fileInfo.Name);
                if (member.IsValid)
                    members.Add(member.DiscordUser.Id, member);
            }
        }

        public void SaveMembers(object _object = null)
        {
            if (!Directory.Exists("Save/Members"))
                Directory.CreateDirectory("Save/Members");

            foreach (Member member in members.Values)
                member.Save();
        }

        public void Start()
        {
            timer.Change(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
        }

        public void Stop()
        {
            timer.Dispose();
            SaveMembers();
        }

        public void DisconnectEvents()
        {
            Bot.DiscordClient.UserVoiceStateUpdated -= OnUserVoiceStateUpdated;
        }

        public void ConnectEvents()
        {
            Bot.DiscordClient.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
        }
    }
}
