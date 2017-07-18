using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using DiscordBot.Sym;

namespace DiscordBot
{
    internal static class ProcessMessage
    {
        public static readonly CommandList CommandList = new CommandList();
        private static readonly ReactionProcess Reaction = new ReactionProcess();

        private enum EAnswerType
        {
            Command,
            Reaction,
            Any
        }

        public static void Process(SocketMessage _message)
        {
            EAnswerType answerType = GetMessageType(_message);

            switch (answerType)
            {
                case EAnswerType.Command:
                    CommandList.ExecuteCommand(_message);
                    break;
                case EAnswerType.Reaction:
                    Reaction.Process(_message);
                    break;
                case EAnswerType.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static EAnswerType GetMessageType(SocketMessage _message)
        {
            SocketMessage message = _message;

            if(message.Content.StartsWith("!"))
                return EAnswerType.Command;
            if (message.MentionedUsers.Any(_user => _user.Id == Bot.DiscordClient.CurrentUser.Id) || !(_message.Channel is IGuildChannel))
                return EAnswerType.Reaction;

            return EAnswerType.Any;
        }
    }
}
