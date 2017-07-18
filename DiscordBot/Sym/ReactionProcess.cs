using System.IO;
using Discord;
using Discord.WebSocket;
using Syn.Bot.Siml;

namespace DiscordBot.Sym
{
    internal class ReactionProcess
    {
        private readonly SimlBot synBot = new SimlBot();
        private readonly BotUser synUser;

        public ReactionProcess()
        {
            synUser = synBot.MainUser;

            string packageString = File.ReadAllText(@"PacBot.simlpk");
            synBot.PackageManager.LoadFromString(packageString);
        }

        public async void Process(SocketMessage _message)
        {
            string message = PrepareString(_message);

            ChatRequest request = new ChatRequest(message, synUser);
            ChatResult chatResult = synBot.Chat(request);
            string botMessage = chatResult.BotMessage;

            if (chatResult.Success)
                await _message.Channel.SendMessageAsync(botMessage);
        }

        private string PrepareString(SocketMessage _message)
        {
            string message = _message.Content;

            if (_message.Channel is IGuildChannel)
            {
                message = message.Replace("<@210151124139900928>", "");
            }

            return message;
        }
    }
}
