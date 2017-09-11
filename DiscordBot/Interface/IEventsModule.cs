namespace DiscordBot.Interface
{
    public interface IEventsModule : IModule
    {
        void DisconnectEvents();
        void ConnectEvents();
    }
}