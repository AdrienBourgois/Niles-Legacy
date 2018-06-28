using System;
using System.Collections.Generic;
using System.Threading;
using Niles.Interface;
using Niles.Modules.Channels;
using Niles.Modules.Commands;
using Niles.Modules.Members;
using Niles.Modules.Voice;

namespace Niles
{
    internal class ModuleManager
    {
        private readonly Type[] modulesList =
        {
            typeof(CommandManager),
            typeof(ChannelManager),
            typeof(MemberManager),
            typeof(MusicManager)
        };

        private static readonly List<IModule> modules = new List<IModule>();
        private readonly List<IRealTimeModule> realTimemodules = new List<IRealTimeModule>();

        public static T GetModule<T>() where T : IModule
        {
            foreach (IModule module in modules)
            {
                if (module is T t)
                    return t;
            }

            return default;
        }

        public void Start()
        {
            foreach (Type module_type in modulesList)
            {
                IModule module = (IModule)Activator.CreateInstance(module_type);
                module.Start();
                modules.Add(module);
                if (module is IRealTimeModule real_time_module)
                    realTimemodules.Add(real_time_module);
                if (module is IEventsModule events_module)
                    events_module.ConnectEvents();
            }
        }

        public void Update()
        {
            while (Bot.State == Bot.EBotState.Running || Bot.State == Bot.EBotState.Sleep)
            {
                if (Bot.State != Bot.EBotState.Running) continue;

                foreach (IRealTimeModule module in realTimemodules)
                {
                    module.Update();
                }

                Thread.Sleep(1000);
            }
        }

        public void Sleep()
        {
            foreach (IModule module in modules)
            {
                if(module is IEventsModule events_module)
                    events_module.DisconnectEvents();
            }
        }

        public void Awake()
        {
            foreach (IModule module in modules)
            {
                if (module is IEventsModule events_module)
                    events_module.ConnectEvents();
            }
        }

        public void Shutdown()
        {
            foreach (IModule module in modules)
            {
                module.Stop();
                Bot.State = Bot.EBotState.ReadyToShutdown;
            }
        }
    }
}
