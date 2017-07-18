using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot
{
    internal class DifferedMessage
    {
        public DifferedMessage(string _id, TimeSpan _expirationTime, RestUserMessage _requestMessage, ISocketMessageChannel _channel, List<Action<SocketMessage, string>> _actions)
        {
            Id = _id;
            expirationTime = DateTime.Now.Add(_expirationTime);
            requestMessage = _requestMessage;
            actions = _actions;
            channel = _channel;
        }

        public async Task<bool> HadExpired()
        {
            if (expirationTime > DateTime.Now) return false;

            await channel.SendMessageAsync("Request : " + Id + " expired !");
            await requestMessage.DeleteAsync();
            return true;
        }

        public void Execute()
        {
            foreach (Action<SocketMessage, string> action in actions)
            {
                action(null, null);
            }
        }

        private readonly DateTime expirationTime;
        private readonly RestUserMessage requestMessage;
        private readonly ISocketMessageChannel channel;
        private readonly List<Action<SocketMessage, string>> actions;
        public string Id { get; }
    }
}
