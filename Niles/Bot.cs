using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Niles
{
    public static class Bot
    {
        public static readonly DiscordSocketClient DiscordClient = new DiscordSocketClient();

        private static readonly ModuleManager modules = new ModuleManager();

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

            modules.Start();

            State = EBotState.Running;

            modules.Update();

            modules.Shutdown();
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
                    await DiscordClient.GetUser(ulong.Parse(Data.Configuration["MasterId"])).GetOrCreateDMChannelAsync()
                        .Result.SendMessageAsync("Send from " + _message.Author + " : " + _message.Content);
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

            modules.Sleep();
        }

        private static void WakeUp()
        {
            State = EBotState.Running;

            ConnectEvents();

            DiscordClient.MessageReceived -= MessageReceivedOnSleep;

            modules.Awake();
        }

        private static void ConnectEvents()
        {
            DiscordClient.Log += Log;
            DiscordClient.MessageReceived += OnMessageReceived;
            DiscordClient.Ready += OnReady;
            //DiscordClient.UserJoined += OnUserJoined;
        }

        private static void DisconnectEvents()
        {
            DiscordClient.Log -= Log;
            DiscordClient.MessageReceived -= OnMessageReceived;
            DiscordClient.Ready -= OnReady;
            //DiscordClient.UserJoined -= OnUserJoined;
        }

        private static Task Log(LogMessage _msg)
        {
            Console.WriteLine(_msg.ToString());
            return null;
        }

        /*internal static async void SendToLog(SocketMessage _message, string _text, InputCommand _inputs = null)
        {
            if (_text == null)
                return;

            string replace = _text;
            if (_text.Contains("{USER}"))
                replace = replace.Replace("{USER}", _message.Author.Username);
            if (_inputs != null)
                if (_text.Contains("{PARAMETER}"))
                    replace = replace.Replace("{PARAMETER}", _inputs.Sentence);

            foreach (SocketTextChannel socket_text_channel in Data.Guild.TextChannels)
            {
                if (socket_text_channel.Name != Data.Configuration["LogsChannelName"]) continue;

                await socket_text_channel.SendMessageAsync(replace);
                break;
            }
        }*/
        //TODO : Rework Log messages with ParsedMessage
    }
}
