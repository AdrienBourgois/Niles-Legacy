using System.Threading.Tasks;
using DiscordBot.Managers;

namespace DiscordBot
{
    internal class RealTimeUpdate
    {
        private static DifferedMessagesManager DifferedMessageManager { get; } = new DifferedMessagesManager();
        public static ChannelManager ChannelManager { get; } = new ChannelManager();

        public async Task StartUpdate()
        {
            while (Bot.State == Bot.EBotState.Running)
            {
                await Update();
            }
        }

        private async Task Update()
        {
            DifferedMessageManager.Update();
            await ChannelManager.Update();
        }
    }
}
