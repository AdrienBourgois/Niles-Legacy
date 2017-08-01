using System;
using System.Collections.Generic;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Interface;

namespace DiscordBot
{
    internal class DifferedMessagesManager : IModule
    {
        public bool IsRealTime { get; set; } = true;

        private static int currentId = 1;

        private static readonly Dictionary<string, DifferedMessage> DifferedMessages = new Dictionary<string, DifferedMessage>();

        private readonly List<string> messagesToRemove = new List<string>();

        public static async void AddDifferedMessage(SocketMessage _message, int _timer)
        {
            ISocketMessageChannel channel = _message.Channel;
            await _message.DeleteAsync();
            await channel.SendMessageAsync("Differed Message Added !");

            string id = GetNewId();
            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != "staff") continue;

                RestUserMessage sendMessage = await socketTextChannel.SendMessageAsync(id + " Request");
                await sendMessage.AddReactionAsync(new Emoji("✅"));
                await sendMessage.AddReactionAsync(new Emoji("❎"));

                DifferedMessages.Add(id, new DifferedMessage(id, TimeSpan.FromSeconds(_timer), sendMessage, socketTextChannel, new List<Action<SocketMessage, string>> { BotFunctions.Reply }));

                break;
            }
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Update()
        {
            VerifyDifferedMessagesTimers();
        }

        private void VerifyDifferedMessagesTimers()
        {
            if (DifferedMessages.Count == 0) return;

            foreach (DifferedMessage differedMessage in DifferedMessages.Values)
            {
                if (differedMessage.HadExpired().Result)
                    messagesToRemove.Add(differedMessage.Id);
            }

            RemoveExpiredMessages();
        }

        private void RemoveExpiredMessages()
        {
            foreach (string s in messagesToRemove)
            {
                DifferedMessages.Remove(s);
            }
            messagesToRemove.Clear();
        }

        private static string GetNewId()
        {
            string id = DateTime.Now.ToString("Mddhhmm") + "-" + currentId;
            ++currentId;
            return id;
        }
    }
}
