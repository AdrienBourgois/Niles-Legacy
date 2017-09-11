using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Types;

namespace DiscordBot.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    internal static class CommandsFunctions
    {
        public static async void SendHelpList(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> commandPair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = commandPair.Value;
                if (!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "\n```";

            await _message.Channel.SendMessageAsync(answer);
        }

        public static async void SendAdminHelpList(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            string answer = "Liste des commandes : \n```\n";
            foreach (KeyValuePair<string, Command> commandPair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = commandPair.Value;
                if (!command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }
            answer += "```\nListe des commandes administateurs : \n```\n";

            foreach (KeyValuePair<string, Command> commandPair in ModuleManager.GetModule<CommandManager>().Commands)
            {
                Command command = commandPair.Value;
                if (command.AdminCommand)
                    answer += "- !" + command.Name + " : " + command.Description + '\n';
            }

            answer += "\n```";

            await _message.Channel.SendMessageAsync(answer);
        }

        public static async void GetFunctionsList(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            string answer = "Liste des méthodes : \n";

            foreach (Type classType in CommandManager.FunctionsClassesList)
            {
                answer += "**" + classType.Name + "**\n```\n";
                answer += classType.GetMethods(BindingFlags.Static | BindingFlags.Public).Aggregate("", (_current, _method) => _current + (_method.Name + '\n'));
                answer += "\n```";
            }

            await _message.Channel.SendMessageAsync(answer);
        }
    }
}
