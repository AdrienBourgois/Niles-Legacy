using System.Collections.Generic;
using Discord.WebSocket;

namespace Niles.Types
{
    internal readonly struct ParsedMessage
    {
        public readonly SocketMessage Message;
        public readonly string Sentence;
        public readonly char Discriminator;
        public readonly string CommandName;
        public readonly List<string> Parameters;
        public readonly string ConfigParameter;

        public ParsedMessage(SocketMessage _message, string _configParameter)
        {
            Message = _message;
            Discriminator = _message.Content[0];
            Parameters = new List<string>();

            string[] split = _message.Content.Split(' ');
            CommandName = split[0].Substring(1);

            foreach (string s in split)
            {
                if (s == split[0]) continue;
                Parameters.Add(s);
            }

            Sentence = _message.Content.Contains(" ") ? _message.Content.Substring(_message.Content.IndexOf(' ') + 1) : string.Empty;
            ConfigParameter = _configParameter;
        }
    }
}
