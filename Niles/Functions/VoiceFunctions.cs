using System.Linq;
using Discord.WebSocket;
using Niles.Modules.Voice;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal class VoiceFunctions
    {
        public static async void ConnectVoice(ParsedMessage _message)
        {
            if (string.IsNullOrEmpty(_message.Sentence))
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Error : Any channel name provided");
                return;
            }

            SocketVoiceChannel channel = Data.Guild.VoiceChannels.First(_x => _x.Name == _message.Sentence);
            if (channel == null)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Error : Any channel with this name");
                return;
            }
            ModuleManager.GetModule<MusicManager>().Connect(channel);
        }

        public static void DisconnectVoice(ParsedMessage _message)
        {
            ModuleManager.GetModule<MusicManager>().Disconnect();
        }

        public static async void PlayMusic(ParsedMessage _message)
        {
            if (_message.Parameters.Count == 0)
            {
                await _message.Message.Channel.SendMessageAsync(":negative_squared_cross_mark: Error : Any music provided");
                return;
            }

            await ModuleManager.GetModule<MusicManager>().SendAudioAsync(_message.Parameters[0]);
        }

        public static async void SendMusicList(ParsedMessage _message)
        {
            string music_list = MusicManager.GetMusicList();
            await _message.Message.Channel.SendMessageAsync("Music List :\n```" + music_list + "```");
        }
    }
}
