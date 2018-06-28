using Niles.Modules.Commands;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal static class BotFunctions
    {
        public static void StopBot(ParsedMessage _message)
        {
            Bot.AskToStop();
        }

        public static async void Sleep(ParsedMessage _message)
        {
            await _message.Message.Channel.SendMessageAsync("A tout à l'heure ! :wave:");
            Bot.Sleep();
        }

        public static void ReloadConfig(ParsedMessage _message)
        {
            ModuleManager.GetModule<CommandManager>().PrepareCommands();
        }

        public static async void SetUsername(ParsedMessage _message)
        {
            await Bot.DiscordClient.CurrentUser.ModifyAsync(_x => _x.Username = _message.Sentence);
        }
    }
}
