using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public static class Bot
    {
        public static readonly DiscordSocketClient DiscordClient = new DiscordSocketClient();

        private static readonly ModuleManager Modules = new ModuleManager();

        public enum EBotState
        {
            Starting,
            Ready,
            Running,
            Sleep,
            AskToShutdown,
            ReadyToShutdown,
            Shutdown,
            Exit
        }

        internal static EBotState State { get; set; }

        public static void Main()
            => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            State = EBotState.Starting;

            Data.LoadConfig();

            ConnectEvents();

            await DiscordClient.LoginAsync(TokenType.Bot, Data.Token);
            State = EBotState.Ready;

            await DiscordClient.StartAsync();
            State = EBotState.Running;

            Modules.Start();

            Modules.Update();

            State = EBotState.Shutdown;

            await Shutdown();

            State = EBotState.Exit;
        }

        private static async Task Shutdown()
        {
            Modules.Shutdown();
            await DiscordClient.LogoutAsync();
            await DiscordClient.StopAsync();
        }

        private static async Task OnUserJoined(SocketGuildUser _socketGuildUser)
        {
            await _socketGuildUser.GetOrCreateDMChannelAsync().Result.SendMessageAsync(Data.Configuration["WelcomeMessage"]);
            await Data.Guild.TextChannels.First(_x => _x.Name.Contains(Data.Configuration["GeneralChannelName"])).SendMessageAsync("On souhaite la bienvenue à " + _socketGuildUser.Username + " !");
        }

#pragma warning disable CS1998
        private static async Task OnVoiceStateUpdated(SocketUser _socketUser, SocketVoiceState _fromState, SocketVoiceState _toState)
        {
            SocketGuildUser user = Data.Guild.GetUser(_socketUser.Id);

            if(user.Roles.Count == 1 && user.Roles.First().IsEveryone)
                if (_fromState.VoiceChannel != _toState.VoiceChannel)
                    if(_toState.VoiceChannel != null)
                        if (_toState.VoiceChannel.Name.Contains(Data.Configuration["AccueilVoiceChannelName"]) && _toState.VoiceChannel.Users.Count == 1)
                            BotFunctions.SendToConnectedStaffMembers(null, _socketUser.Username + " vient de se connecter sur l'accueil ! Il est tout seul, ce serait cool de venir l'accueillir !");
        }

        private static async Task OnMessageReceived(SocketMessage _message)
        {
            if(!_message.Author.IsBot)
            {
                if (!(_message.Channel is IGuildChannel) && _message.Author.Id != ulong.Parse(Data.Configuration["MasterId"]))
                    BotFunctions.SendToMaster(_message, null);
                ProcessMessage.Process(_message);
            }
        }

        private static async Task MessageReceivedOnSleep(SocketMessage _message)
        {
            if (_message.Content == "!wakeUp" && Tools.IsFromAdmin(_message))
                WakeUp();
        }

        private static async Task OnReady()
        {
            Data.Guild = DiscordClient.GetGuild(ulong.Parse(Data.Configuration["GuildId"]));
            await DiscordClient.SetGameAsync("Version Bêta");
        }
#pragma warning restore CS1998

        public static void AskToStop()
        {
            DisconnectEvents();
            Thread.Sleep(500);
            State = EBotState.AskToShutdown;
        }

        public static void Sleep()
        {
            State = EBotState.Sleep;

            DisconnectEvents();

            DiscordClient.MessageReceived += MessageReceivedOnSleep;
        }

        private static void WakeUp()
        {
            State = EBotState.Running;

            ConnectEvents();

            DiscordClient.MessageReceived -= MessageReceivedOnSleep;
        }

        private static void ConnectEvents()
        {
            DiscordClient.Log += Log;
            DiscordClient.MessageReceived += OnMessageReceived;
            DiscordClient.Ready += OnReady;
            DiscordClient.UserVoiceStateUpdated += OnVoiceStateUpdated;
            DiscordClient.UserJoined += OnUserJoined;
        }

        private static void DisconnectEvents()
        {
            DiscordClient.Log -= Log;
            DiscordClient.MessageReceived -= OnMessageReceived;
            DiscordClient.Ready -= OnReady;
            DiscordClient.UserVoiceStateUpdated -= OnVoiceStateUpdated;
            DiscordClient.UserJoined -= OnUserJoined;
        }

        private static Task Log(LogMessage _msg)
        {
            Console.WriteLine(_msg.ToString());
            return null;
        }
    }
}
