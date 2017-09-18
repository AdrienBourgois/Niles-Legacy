using System;
using System.IO;
using System.Linq;
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
            Running,
            Sleep,
            AskToShutdown,
            ReadyToShutdown
        }

        internal static EBotState State { get; set; }

        public static void Main()
        {
            if (!File.Exists("Config/Token.txt"))
            {
                Console.WriteLine("Bot don't have any token, can't start...");
                return;
            }

            Data.Load();
            ConnectEvents();

            StartClientAsync().GetAwaiter().GetResult();
            Modules.Start();

            State = EBotState.Running;

            Modules.Update();

            Modules.Shutdown();
            StopClientAsync().GetAwaiter().GetResult();
        }

        private static async Task StartClientAsync()
        {
            await DiscordClient.LoginAsync(TokenType.Bot, File.ReadAllText("Config/Token.txt"));
            await DiscordClient.StartAsync();
        }

        private static async Task StopClientAsync()
        {
            await DiscordClient.LogoutAsync();
            await DiscordClient.StopAsync();
        }

        private static async Task OnUserJoined(SocketGuildUser _socketGuildUser)
        {
            if (_socketGuildUser.Guild.Id != Data.Guild.Id) return;

            await _socketGuildUser.GetOrCreateDMChannelAsync().Result.SendMessageAsync(Data.Configuration["WelcomeMessage"]);
            await Data.Guild.TextChannels.First(_x => _x.Name.Contains(Data.Configuration["GeneralChannelName"])).SendMessageAsync(Data.GetRandomSentence(Data.SentenceType.Welcome, "USER", _socketGuildUser.Username));
        }

#pragma warning disable CS1998
        private static async Task OnMessageReceived(SocketMessage _message)
        {
            if(!_message.Author.IsBot)
            {
                if (!(_message.Channel is IGuildChannel) && _message.Author.Id != ulong.Parse(Data.Configuration["MasterId"]))
                    MessagesFunctions.SendToMaster(_message, null);

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
                    replace = replace.Replace("{PARAMETER}", _inputs.Sentence);

            foreach (SocketTextChannel socketTextChannel in Data.Guild.TextChannels)
            {
                if (socketTextChannel.Name != Data.Configuration["LogsChannelName"]) continue;

                await socketTextChannel.SendMessageAsync(replace);
                break;
            }
        }
    }
}
