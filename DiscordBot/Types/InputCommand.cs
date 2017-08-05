using System.Collections.Generic;
using Discord;

namespace DiscordBot.Types
{
    internal class InputCommand
    {
        public readonly char Discriminator;
        public readonly string CommandName;
        public readonly List<string> Parameters = new List<string>();
        public readonly string Sentence;

        public InputCommand(IMessage _message)
        {
            Discriminator = _message.Content[0];

            string[] split = _message.Content.Split(' ');
            CommandName = split[0].Substring(1);

            foreach (string s in split)
            {
                if (s == split[0]) continue;
                Parameters.Add(s);
            }

            if (_message.Content.Contains(" "))
                Sentence = _message.Content.Substring(_message.Content.IndexOf(' ') + 1);
        }
    }
}
