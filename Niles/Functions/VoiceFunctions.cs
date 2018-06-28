using Niles.Modules.Voice;
using Niles.Types;

namespace Niles.Functions
{
    [FunctionClass]
    internal class VoiceFunctions
    {
        public static void ConnectVoice(ParsedMessage _message)
        {
            ModuleManager.GetModule<MusicManager>().Connect();
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
