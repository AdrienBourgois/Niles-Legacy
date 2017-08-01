namespace DiscordBot.Interface
{
    public interface IModule
    {
        void Start();

        void Stop();

        void Update();

        bool IsRealTime { get; set; }
    }
}