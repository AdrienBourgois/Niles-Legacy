using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Interface;

namespace DiscordBot.Modules
{
    internal class ChannelManager : IRealTimeModule
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

        public async void Update()
        {
            await VerifyVoidChannels();
        }

        private async Task VerifyVoidChannels()
        {
            foreach (KeyValuePair<ulong, IVoiceChannel> channelPair in temporaryChannels)
            {
                SocketVoiceChannel channel = Data.Guild.GetVoiceChannel(channelPair.Value.Id);
                if (channel.Users.Count != 0 || DateTimeOffset.Now < channel.CreatedAt.AddMinutes(1)) continue;

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
