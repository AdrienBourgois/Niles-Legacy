using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Discord;
using Discord.WebSocket;
using DiscordBot.Interface;
using DiscordBot.Types;

namespace DiscordBot.Managers
{
    internal class CommandManager : IModule
    {
        public CommandManager()
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
                        Action<SocketMessage, string, char, string, List<string>> action = null;
                        foreach (MethodInfo methodInfo in botTasksList)
                            if (methodInfo.Name == xmlReader.GetAttribute("function"))
                                action = (Action<SocketMessage, string, char, string, List<string>>)methodInfo.CreateDelegate(typeof(Action<SocketMessage, string, char, string, List<string>>));

                        string actionLog = xmlReader.GetAttribute("log");

                        xmlReader.Read();
                        typeof(BotFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        string value = xmlReader.Value;
                        if (value == "\n    ") value = null;
                        command.AddCommand(action, value, actionLog);
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "List")
                    {
                        Commands.Add(name, command);
                        break;
                    }
                }
            }

            xmlReader.Dispose();
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
                if(command.CommandLog != "")
                    BotFunctions.SendToLog(_message, command.CommandLog);

            foreach (BotTask action in command.ActionList)
            {
                action.Execute(_message);
            }
        }

        public void Start()
        {
            PrepareCommands();
        }

        public void Stop()
        {

        }
    }
}