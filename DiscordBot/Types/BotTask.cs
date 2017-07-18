using System;
using Discord.WebSocket;

namespace DiscordBot.Types
{
    internal class BotTask
    {
        public BotTask(Action<SocketMessage, string> _task, string _text)
        {
            task = _task;
            text = _text;
        }

        public void Execute(SocketMessage _message)
        {
            task(_message, text);
        }

        private readonly Action<SocketMessage, string> task;
        private readonly string text;
    }
}
