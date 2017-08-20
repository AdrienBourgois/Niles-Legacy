using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Interface;
using DiscordBot.Types;

namespace DiscordBot.Managers
{
    internal class MembersManagers : IModule
    {
        private readonly Dictionary<ulong, Member> members = new Dictionary<ulong, Member>();

        public MembersManagers()
        {
            Bot.DiscordClient.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
            Bot.DiscordClient.Ready += delegate { LoadMembers(); return null; };
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

        public void SaveMembers()
        {
            if (Directory.Exists("Save/Members"))
            {
                DirectoryInfo memberDirectory = new DirectoryInfo("Save/Members");
                foreach (FileInfo fileInfo in memberDirectory.GetFiles())
                    fileInfo.Delete();
            }
            else
                Directory.CreateDirectory("Save/Members");

            foreach (Member member in members.Values)
                member.Save();
        }

        public void Start()
        {

        }

        public void Stop()
        {
            SaveMembers();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool IsRealTime { get; set; } = false;
    }
}
