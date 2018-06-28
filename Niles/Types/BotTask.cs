using System;
using Discord.WebSocket;

namespace Niles.Types
{
    internal class BotTask
    {
        public BotTask(Action<ParsedMessage> _task, string _configText = null, string _log = null)
        {
            task = _task;
            configText = _configText;
            log = _log;
        }

        public void Execute(SocketMessage _message)
        {
            task(new ParsedMessage(_message, configText));

            //if (log != null)
                //Bot.SendToLog(_message, log, input);
            //TODO : Rework with new input class
        }

        private readonly Action<ParsedMessage> task;
        private readonly string configText;
        private readonly string log;
    }
}
