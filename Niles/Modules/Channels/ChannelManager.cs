using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Niles.Interface;

namespace Niles.Modules.Channels
{
    internal class ChannelManager : IEventsModule
    {
        private readonly ConcurrentDictionary<ulong, IVoiceChannel> temporaryChannels = new ConcurrentDictionary<ulong, IVoiceChannel>();

        public async void CreateChannel(SocketUser _user, string _name)
        {
            RestVoiceChannel channel = await Data.Guild.CreateVoiceChannelAsync(_name);

            SocketGuildUser guild_user = _user as SocketGuildUser;

            if (guild_user?.VoiceChannel != null)
                await guild_user.ModifyAsync(_x => _x.Channel = channel);
            temporaryChannels.TryAdd(channel.Id, channel);
        }

        private async Task VerifyVoidChannels(SocketUser _socketUser, SocketVoiceState _fromState, SocketVoiceState _toState)
        {
            if (_fromState.VoiceChannel != null && _fromState.VoiceChannel.Guild == Data.Guild && temporaryChannels.ContainsKey(_fromState.VoiceChannel.Id))
            {
                SocketVoiceChannel channel = _fromState.VoiceChannel;
                if (channel.Users.Count != 0) return;

                await channel.DeleteAsync();
                temporaryChannels.TryRemove(channel.Id, out IVoiceChannel _);
            }
        }

        public void Start()
        {}

        public async void Stop()
        {
            foreach (KeyValuePair<ulong, IVoiceChannel> channel_pair in temporaryChannels)
            {
                SocketVoiceChannel channel = Data.Guild.GetVoiceChannel(channel_pair.Value.Id);
                if (channel.Users.Count == 0) continue;

                SocketVoiceChannel move_channel = null;
                foreach (SocketVoiceChannel socket_voice_channel in Data.Guild.VoiceChannels)
                {
                    if (socket_voice_channel.Name.Contains(Data.Configuration["AccueilVoiceChannelName"]))
                        move_channel = socket_voice_channel;
                }

                foreach (SocketGuildUser user in channel.Users)
                {
                    await user.ModifyAsync(_x => _x.Channel = move_channel);
                }

                await channel.DeleteAsync();
            }
        }

        public void DisconnectEvents()
        {
            Bot.DiscordClient.UserVoiceStateUpdated -= VerifyVoidChannels;
        }

        public void ConnectEvents()
        {
            Bot.DiscordClient.UserVoiceStateUpdated += VerifyVoidChannels;
        }
    }
}
