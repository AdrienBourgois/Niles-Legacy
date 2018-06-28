using System;
using System.Linq;
using Discord.Audio;
using Discord.WebSocket;
using Niles.Interface;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Niles.Modules.Voice
{
    internal class MusicManager : IEventsModule
    {
        private SocketVoiceChannel currentChannel;
        private IAudioClient client;

        public async void Connect()
        {
            if (client != null) return;
            currentChannel = Data.Guild.VoiceChannels.First();
            client = await currentChannel.ConnectAsync();
        }

        public async void Disconnect()
        {
            if(client != null)
                await client.StopAsync();
            client = null;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            Disconnect();
        }

        public void DisconnectEvents()
        {
        }

        public void ConnectEvents()
        {
        }

        public async Task SendAudioAsync(string _path)
        {
            if (client == null) return;

            _path = _path.Insert(0, "Music/");

            if (!File.Exists(_path))
            {
                Console.WriteLine("File does not exist.");
                return;
            }

            using (Process ffmpeg = CreateStream(_path))
            using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
        }

        private static Process CreateStream(string _path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{_path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public static string GetMusicList()
        {
            return !Directory.Exists("Music") ? string.Empty : string.Join('\n', Directory.GetFiles("Music", "*.mp3"));
        }
    }
}
