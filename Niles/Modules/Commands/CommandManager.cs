using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Discord;
using Discord.WebSocket;
using Niles.Interface;
using Niles.Types;

namespace Niles.Modules.Commands
{
    internal class CommandManager : IEventsModule
    {
        private int runningCommands;

        internal static Type[] FunctionsClassesList;

        public CommandManager()
        {
            FunctionsClassesList = Utilities.GetTypesWithAttribute<FunctionClassAttribute>();
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

            foreach (Type function_class in FunctionsClassesList)
                methods.AddRange(function_class.GetMethods(BindingFlags.Static | BindingFlags.Public));

            XmlDocument document = new XmlDocument();
            XmlSchema schema = XmlSchema.Read(System.IO.File.OpenRead("Config/Commands/CommandsSchema.xsd"), null);
            document.Schemas.Add(schema);
            document.Schemas.Compile();
            document.Load("Config/Commands/BaseCommands.xml");
            document.Validate(delegate(object _sender, ValidationEventArgs _args) { Console.WriteLine("PrepareCommand : " + _args.Message); });

            foreach (XmlNode command_node in document["CommandList"].ChildNodes)
            {
                if (command_node.Name != "Command") continue;

                string name = command_node.Attributes["name"].Value;
                string description = command_node.Attributes["description"]?.Value;
                string command_log = command_node.Attributes["log"]?.Value;
                bool admin = command_node.Attributes["admin"]?.Value == "true";
                Command command = new Command(name, description, command_log, admin);

                foreach (XmlNode action_node in command_node.ChildNodes)
                {
                    if (action_node.Name != "Action") continue;

                    MethodInfo method = methods.Find(_method => _method.Name == action_node.Attributes["function"].Value);
                    Action<ParsedMessage> action = (Action<ParsedMessage>)method.CreateDelegate(typeof(Action<ParsedMessage>));

                    string action_log = action_node.Attributes["log"]?.Value;
                    string value = string.IsNullOrEmpty(action_node.InnerText) ? null : action_node.InnerText;
                    command.AddCommand(action, value, action_log);
                }

                Commands.Add(name, command);
            }
        }

        private void ExecuteCommand(SocketMessage _message)
        {
            string[] message_split = _message.Content.Substring(1).Split(' ');

            if (!Commands.ContainsKey(message_split[0])) return;

            runningCommands++;

            Command command = Commands[message_split[0]];

            if (_message.Source != MessageSource.User)
                return;

            if (command.AdminCommand)
            {
                if (!Tools.IsFromAdmin(_message))
                    return;
            }
            /*else
                if(command.CommandLog != null)
                    Bot.SendToLog(_message, command.CommandLog);*/
            //TODO : Enable with new Log messages

            foreach (BotTask action in command.ActionList)
            {
                action.Execute(_message);
            }

            runningCommands--;
        }

        public void Start()
        {
            PrepareCommands();
        }

        public void Stop()
        {
            while (runningCommands != 0)
            {}
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