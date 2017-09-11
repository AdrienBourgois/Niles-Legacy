using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Interface;

namespace DiscordBot.Modules
{
    internal class ChannelManager : IEventsModule
    {
        private readonly ConcurrentDictionary<ulong, IVoiceChannel> temporaryChannels = new ConcurrentDictionary<ulong, IVoiceChannel>();

        public async void CreateChannel(SocketUser _user, string _name)
        {
            RestVoiceChannel channel = await Data.Guild.CreateVoiceChannelAsync(_name);

            SocketGuildUser guildUser = _user as SocketGuildUser;

            if (guildUser?.VoiceChannel != null)
                await guildUser.ModifyAsync(_x => _x.Channel = channel);
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
            foreach (KeyValuePair<ulong, IVoiceChannel> channelPair in temporaryChannels)
            {
                SocketVoiceChannel channel = Data.Guild.GetVoiceChannel(channelPair.Value.Id);
                if (channel.Users.Count == 0) continue;

                SocketVoiceChannel moveChannel = null;
                foreach (SocketVoiceChannel socketVoiceChannel in Data.Guild.VoiceChannels)
                {
                    if (socketVoiceChannel.Name.Contains(Data.Configuration["AccueilVoiceChannelName"]))
                        moveChannel = socketVoiceChannel;
                }

                foreach (SocketGuildUser user in channel.Users)
                {
                    await user.ModifyAsync(_x => _x.Channel = moveChannel);
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
