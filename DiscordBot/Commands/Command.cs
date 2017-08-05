﻿using System;
using System.Collections.Generic;
using Discord.WebSocket;
using DiscordBot.Types;

namespace DiscordBot
{
    internal class Command
    {
        public Command(string _name, string _description, string _logMessage, bool _adminCommand)
        {
            Name = _name;
            Description = _description;
            LogMessage = _logMessage;
            AdminCommand = _adminCommand;
        }

        public void AddCommand(Action<SocketMessage, string, char, string, List<string>> _action, string _parameter, string _log = null)
        {
            if(_action != null)
                ActionList.Add(new BotTask(_action, _parameter, _log));
        }

        public readonly string Name;
        public readonly string Description;
        public readonly string LogMessage;
        public bool AdminCommand { get; }
        public readonly List<BotTask> ActionList = new List<BotTask>();
    }
}
