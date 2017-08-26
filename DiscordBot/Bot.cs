using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Functions;
using DiscordBot.Sym;
using DiscordBot.Types;

namespace DiscordBot
{
    public static class Bot
    {
        public static readonly DiscordSocketClient DiscordClient = new DiscordSocketClient();

        private static readonly ModuleManager Modules = new ModuleManager();

        private static readonly ReactionProcess Reaction = new ReactionProcess();

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
        private static async Task OnMessageReceived(SocketMessage _message)
        {
            if(!_message.Author.IsBot)
            {
                if (!(_message.Channel is IGuildChannel) && _message.Author.Id != ulong.Parse(Data.Configuration["MasterId"]))
                    BaseFunctions.SendToMaster(_message, null);

                if (_message.MentionedUsers.Any(_user => _user.Id == DiscordClient.CurrentUser.Id) || !(_message.Channel is IGuildChannel))
                    Reaction.Process(_message);
            }
        }

        private static async Task MessageReceivedOnSleep(SocketMessage _message)
        {
            if (_message.Content == "!wakeUp" && Tools.IsFromAdmin(_message))
            {
                await _message.Channel.SendMessageAsync("De retour ! :ok_hand:");
                WakeUp();
            }
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

            Modules.Sleep();
        }

        private static void WakeUp()
        {
            State = EBotState.Running;

            ConnectEvents();

            DiscordClient.MessageReceived -= MessageReceivedOnSleep;

            Modules.Awake();
        }

        private static void ConnectEvents()
        {
            DiscordClient.Log += Log;
            DiscordClient.MessageReceived += OnMessageReceived;
            DiscordClient.Ready += OnReady;
            DiscordClient.UserJoined += OnUserJoined;
        }

        private static void DisconnectEvents()
        {
            DiscordClient.Log -= Log;
            DiscordClient.MessageReceived -= OnMessageReceived;
            DiscordClient.Ready -= OnReady;
            DiscordClient.UserJoined -= OnUserJoined;
        }

        private static Task Log(LogMessage _msg)
        {
            Console.WriteLine(_msg.ToString());
            return null;
        }

        internal static async void SendToLog(SocketMessage _message, string _text, InputCommand _inputs = null)
        {
            if (_text == null)
                return;

            string replace = _text;
            if (_text.Contains("{USER}"))
                replace = replace.Replace("{USER}", _message.Author.Username);
            if (_inputs != null)
                if (_text.Contains("{PARAMETER}"))
                    replace = replace.Replace("{PARAMETER}", _inputs.Parameters.Aggregate((_i, _j) => _i + ", " + _j));

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["LogsChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(replace);
                break;
            }
        }
    }
}
