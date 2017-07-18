using System;
using Discord.WebSocket;

namespace DiscordBot.Types
{
    internal class BotTask
    {
        public BotTask(Action<SocketMessage, string> _task, string _text, string _log = null)
        {
            task = _task;
            text = _text;
            log = _log;
        }

        public void Execute(SocketMessage _message)
        {
            task(_message, text);
            BotFunctions.SendToLog(_message, log, null);
        }

        private readonly Action<SocketMessage, string> task;
        private readonly string text;
        private readonly string log;
    }
}
