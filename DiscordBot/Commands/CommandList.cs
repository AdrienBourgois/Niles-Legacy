using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Discord;
using Discord.WebSocket;
using DiscordBot.Types;

namespace DiscordBot
{
    internal class CommandList
    {
        public CommandList()
        {
            PrepareCommands();
        }

        public readonly Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public void PrepareCommands()
        {
            Commands.Clear();

            MethodInfo[] botTasksList = typeof(BotFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public);
            XmlReader xmlReader = XmlReader.Create("Commands/CommandList.xml");

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "Command") continue;
                string name = xmlReader.GetAttribute("name");
                string commandLog = xmlReader.GetAttribute("log");
                string description = xmlReader.GetAttribute("description");
                bool admin = xmlReader.GetAttribute("admin") == "true";
                Command command = new Command(name, description, commandLog, admin);

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Action")
                    {
                        Action<SocketMessage, string> function = null;
                        foreach (MethodInfo methodInfo in botTasksList)
                            if (methodInfo.Name == xmlReader.GetAttribute("function"))
                                function = (Action<SocketMessage, string>) methodInfo.CreateDelegate(typeof(Action<SocketMessage, string>));

                        string actionLog = xmlReader.GetAttribute("log");

                        xmlReader.Read();
                        typeof(BotFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        command.AddCommand(function, xmlReader.Value, actionLog);
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "List")
                    {
                        Commands.Add(name, command);
                        break;
                    }
                }
            }
        }

        public void ExecuteCommand(SocketMessage _message)
        {
            string[] messageSplit = _message.Content.Substring(1).Split(' ');

            if (!Commands.ContainsKey(messageSplit[0])) return;

            Command command = Commands[messageSplit[0]];

            if (_message.Source != MessageSource.User)
                return;

            if (command.AdminCommand)
            {
                if (!Tools.IsFromAdmin(_message))
                    return;
            }
            else
                if(command.LogMessage != "")
                    BotFunctions.SendToLog(_message, command.LogMessage, null);

            foreach (BotTask action in command.ActionList)
            {
                action.Execute(_message);
            }
        }
    }
}