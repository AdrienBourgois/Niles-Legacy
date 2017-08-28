using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Discord;
using Discord.WebSocket;
using DiscordBot.Functions;
using DiscordBot.Interface;
using DiscordBot.Types;

namespace DiscordBot.Modules
{
    internal class CommandManager : IModule, IEventsModule
    {
        private readonly Type[] functionsClassesList =
        {
            typeof(BaseFunctions),
            typeof(MemberFunctions)
        };

        public CommandManager()
        {
            PrepareCommands();
        }

        private Task OnMessageReceived(SocketMessage _message)
        {
            if(_message.Content.StartsWith('!'))
                ExecuteCommand(_message);

            return null;
        }

        public readonly Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public void PrepareCommands()
        {
            Commands.Clear();

            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (Type functionClass in functionsClassesList)
                methods.AddRange(functionClass.GetMethods(BindingFlags.Static | BindingFlags.Public));

            XmlDocument document = new XmlDocument();
            XmlSchema schema = XmlSchema.Read(System.IO.File.OpenRead("Config/Commands/CommandsSchema.xsd"), null);
            document.Schemas.Add(schema);
            document.Schemas.Compile();
            document.Load("Config/Commands/BaseCommands.xml");
            document.Validate(delegate(object _sender, ValidationEventArgs _args) { Console.WriteLine("PrepareCommand : " + _args.Message); });

            foreach (XmlNode commandNode in document["CommandList"].ChildNodes)
            {
                if (commandNode.Name != "Command") continue;

                string name = commandNode.Attributes["name"].Value;
                string description = commandNode.Attributes["description"]?.Value;
                string commandLog = commandNode.Attributes["log"]?.Value;
                bool admin = commandNode.Attributes["admin"]?.Value == "true";
                Command command = new Command(name, description, commandLog, admin);

                foreach (XmlNode actionNode in commandNode.ChildNodes)
                {
                    if (actionNode.Name != "Action") continue;

                    MethodInfo method = methods.Find(_method => _method.Name == actionNode.Attributes["function"].Value);
                    Action<SocketMessage, string, char, string, List<string>> action = (Action<SocketMessage, string, char, string, List<string>>)method.CreateDelegate(typeof(Action<SocketMessage, string, char, string, List<string>>));

                    string actionLog = actionNode.Attributes["log"]?.Value;
                    string value = string.IsNullOrEmpty(actionNode.InnerText) ? null : actionNode.InnerText;
                    command.AddCommand(action, value, actionLog);
                }

                Commands.Add(name, command);
            }
        }

        private void ExecuteCommand(SocketMessage _message)
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
                if(command.CommandLog != null)
                    Bot.SendToLog(_message, command.CommandLog);

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

        public void DisconnectEvents()
        {
            Bot.DiscordClient.MessageReceived -= OnMessageReceived;
        }

        public void ConnectEvents()
        {
            Bot.DiscordClient.MessageReceived += OnMessageReceived;
        }
    }
}