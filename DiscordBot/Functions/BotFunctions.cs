using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    internal static class BotFunctions
    {
        public static void StopBot(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            Bot.AskToStop();
        }

        public static async void Sleep(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            await _message.Channel.SendMessageAsync("A tout à l'heure ! :wave:");
            Bot.Sleep();
        }

        public static void ReloadConfig(SocketMessage _message, string _sentence, char _discriminator = '!', string _commandName = null, List<string> _parameters = null)
        {
            ModuleManager.GetModule<CommandManager>().PrepareCommands();
        }
    }
}
