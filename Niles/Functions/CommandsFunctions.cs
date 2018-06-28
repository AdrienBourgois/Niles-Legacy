using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niles.Modules.Commands;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal static class CommandsFunctions
    {
        public static async void SendHelpList(ParsedMessage _message)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> command_pair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = command_pair.Value;
                if (!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "\n```";

            await _message.Message.Channel.SendMessageAsync(answer);
        }

        public static async void SendAdminHelpList(ParsedMessage _message)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> command_pair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = command_pair.Value;
                if (!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "```\nListe des commandes administateurs : \n```\n";

            foreach (KeyValuePair<string, Command> command_pair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = command_pair.Value;
                if (command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }

            answer += "\n```";

            await _message.Message.Channel.SendMessageAsync(answer);
        }

        public static async void GetFunctionsList(ParsedMessage _message)
        {
            string answer = "Liste des méthodes : \n";

            foreach (Type class_type in CommandManager.FunctionsClassesList)
            {
                answer += "**" + class_type.Name + "**\n```\n";
                answer += class_type.GetMethods(BindingFlags.Static | BindingFlags.Public).Aggregate("", (_current, _method) => _current + (_method.Name + '\n'));
                answer += "\n```";
            }

            await _message.Message.Channel.SendMessageAsync(answer);
        }
    }
}
