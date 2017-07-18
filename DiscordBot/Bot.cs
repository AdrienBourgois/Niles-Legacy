using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public static class Bot
    {
        public static readonly DiscordSocketClient DiscordClient = new DiscordSocketClient();

        private static readonly RealTimeUpdate RealTime = new RealTimeUpdate();

        public enum EBotState
        {
            Starting,
            Ready,
            Running,
            AskToStop,
            Exit
        }

        public static EBotState State { get; private set; }

        public static void Main()
            => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            State = EBotState.Starting;

            Data.LoadConfig();

            DiscordClient.Log += Log;
            DiscordClient.MessageReceived += MessageReceived;
            DiscordClient.Ready += Ready;
            DiscordClient.UserVoiceStateUpdated += VoiceStateUpdated;
            DiscordClient.UserJoined += OnUserJoined;

            await DiscordClient.LoginAsync(TokenType.Bot, Data.Token);
            State = EBotState.Ready;

            await DiscordClient.StartAsync();
            State = EBotState.Running;

            await RealTime.StartUpdate();
            State = EBotState.Exit;
        }

        private static async Task OnUserJoined(SocketGuildUser _socketGuildUser)
        {
            await _socketGuildUser.CreateDMChannelAsync().Result.SendMessageAsync(Data.Configuration["WelcomeMessage"]);
        }

#pragma warning disable CS1998
        private static async Task VoiceStateUpdated(SocketUser _socketUser, SocketVoiceState _fromState, SocketVoiceState _toState)
#pragma warning restore CS1998
        {
            SocketGuildUser user = Data.Guild.GetUser(_socketUser.Id);

            if(user.Roles.Count() == 1 && user.Roles.First().IsEveryone)
                if (_fromState.VoiceChannel != _toState.VoiceChannel)
                    if(_toState.VoiceChannel != null)
                        if (_toState.VoiceChannel.Name.Contains(Data.Configuration["AccueilVoiceChannelName"]) && _toState.VoiceChannel.Users.Count == 1)
                            BotFunctions.SendToConnectedStaffMembers(null, _socketUser.Username + " vient de se connecter sur l'accueil ! Il est tout seul, ce serait cool de venir l'accueillir !");
        }

#pragma warning disable CS1998
        private static async Task MessageReceived(SocketMessage _message)
#pragma warning restore CS1998
        {
            if(!_message.Author.IsBot)
            {
                if (!(_message.Channel is IGuildChannel) && _message.Author.Id != ulong.Parse(Data.Configuration["MasterId"]))
                    BotFunctions.SendToMaster(_message, null);
                Data.Packages.Enqueue(_message);
                ProcessMessage.Process(_message);
            }
        }

#pragma warning disable CS1998
        private static async Task Ready()
#pragma warning restore CS1998
        {
            Data.Guild = DiscordClient.GetGuild(ulong.Parse(Data.Configuration["GuildId"]));
        }

        public static void AskToStop()
        {
            State = EBotState.AskToStop;
        }

        private static Task Log(LogMessage _msg)
        {
            Console.WriteLine(_msg.ToString());
            return Task.CompletedTask;
        }
    }
}
