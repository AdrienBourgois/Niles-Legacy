using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Interface;

namespace DiscordBot.Managers
{
    internal class ChannelManager : IModule
    {
        public bool IsRealTime { get; set; } = true;

        private readonly ConcurrentDictionary<ulong, IVoiceChannel> temporaryChannels = new ConcurrentDictionary<ulong, IVoiceChannel>();

        public async void CreateChannel(SocketGuildUser _user, string _name)
        {
            RestVoiceChannel channel = await Data.Guild.CreateVoiceChannelAsync(_name);

            if (_user.VoiceChannel != null)
                await _user.ModifyAsync(_x => _x.Channel = channel);
            temporaryChannels.TryAdd(channel.Id, channel);
        }

        public async void Update()
        {
            await VerifyVoidChannels();
        }

        private async Task VerifyVoidChannels()
        {
            foreach (KeyValuePair<ulong, IVoiceChannel> channelPair in temporaryChannels)
            {
                SocketVoiceChannel channel = Data.Guild.GetVoiceChannel(channelPair.Value.Id);
                if (channel.Users.Count != 0 || DateTimeOffset.Now < channel.CreatedAt.AddSeconds(5)) continue;

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
    }
}
