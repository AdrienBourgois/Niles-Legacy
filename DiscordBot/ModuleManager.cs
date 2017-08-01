using System;
using System.Collections.Generic;
using System.Threading;
using DiscordBot.Interface;
using DiscordBot.Managers;

namespace DiscordBot
{
    internal class ModuleManager
    {
        private readonly Type[] modulesList =
        {
            typeof(ChannelManager),
            //typeof(DifferedMessagesManager)
        };

        private static readonly List<IModule> Modules = new List<IModule>();
        private readonly List<IModule> realTimemodules = new List<IModule>();

        public static T GetModule<T>() where T : IModule
        {
            foreach (IModule module in Modules)
            {
                if (module is T)
                    return (T) module;
            }

            return default(T);
        }

        public void Start()
        {
            foreach (Type moduleType in modulesList)
            {
                IModule module = (IModule)Activator.CreateInstance(moduleType);
                module.Start();
                Modules.Add(module);
                if(module.IsRealTime)
                    realTimemodules.Add(module);
            }
        }

        public void Update()
        {
            while (Bot.State == Bot.EBotState.Running || Bot.State == Bot.EBotState.Sleep)
            {
                if (Bot.State != Bot.EBotState.Running) continue;

                foreach (IModule module in realTimemodules)
                {
                    module.Update();
                }

                Thread.Sleep(1000);
            }
        }
    }
}
