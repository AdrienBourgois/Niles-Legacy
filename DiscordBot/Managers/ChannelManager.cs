using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot.Managers
{
    internal class ChannelManager
    {
        private readonly ConcurrentDictionary<ulong, SocketVoiceChannel> channels = new ConcurrentDictionary<ulong, SocketVoiceChannel>();


        public async void CreateChannel(SocketGuildUser _user, string _name)
        {
            RestVoiceChannel channel = await Data.Guild.CreateVoiceChannelAsync(_name);
            SocketVoiceChannel socketChannel = null;
            while (socketChannel == null)
            {
                await Task.Delay(25);
                socketChannel = Data.Guild.GetVoiceChannel(channel.Id);
            }
            if (_user.VoiceChannel != null)
                await _user.ModifyAsync(_x => _x.Channel = channel);
            channels.TryAdd(socketChannel.Id, socketChannel);
        }

        public async Task Update()
        {
            await VerifyVoidChannels();
        }

        private async Task VerifyVoidChannels()
        {
            foreach (KeyValuePair<ulong, SocketVoiceChannel> channelPair in channels)
            {
                SocketVoiceChannel channel = channelPair.Value;
                if (channel.Users.Count != 0 || DateTimeOffset.Now < channel.CreatedAt.AddMinutes(1)) continue;

                await channel.DeleteAsync();
                channels.TryRemove(channel.Id, out channel);
            }
        }
    }
}
