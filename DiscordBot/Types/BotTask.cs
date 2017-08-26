using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace DiscordBot.Types
{
    internal class BotTask
    {
        public BotTask(Action<SocketMessage, string, char, string, List<string>> _task, string _configText = null, string _log = null)
        {
            task = _task;
            configText = _configText;
            log = _log;
        }

        public void Execute(SocketMessage _message)
        {
            InputCommand input = new InputCommand(_message);
            task(_message, configText ?? input.Sentence, input.Discriminator, input.CommandName, input.Parameters);

            if (log != null)
                Bot.SendToLog(_message, log, input);
        }

        private readonly Action<SocketMessage, string, char, string, List<string>> task;
        private readonly string configText;
        private readonly string log;
    }
}
