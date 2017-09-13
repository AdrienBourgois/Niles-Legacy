using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Discord.WebSocket;

namespace DiscordBot
{
    internal static class Data
    {
        public static readonly Dictionary<string, string> Configuration = new Dictionary<string, string>();

        private static readonly Dictionary<SentenceType, string[]> Sentences = new Dictionary<SentenceType, string[]>();

        public static string Token { get; } = "(YOUR TOKEN)";

        public static SocketGuild Guild { get; set; }

        public static void Load()
        {
            LoadConfig();
            LoadSentences();
        }

        private static void LoadConfig()
        {
            string content = File.ReadAllText("Config/Config.xml");
            XmlDocument document = new XmlDocument {PreserveWhitespace = false};
            document.LoadXml(content.Trim());

            XmlNode configNode = document.ChildNodes.Cast<XmlNode>().FirstOrDefault(_node => _node.Name == "config");

            if (configNode?.ChildNodes == null) return;

            foreach (XmlNode node in configNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                    Configuration.Add(node.Name, node.InnerText);
            }
        }

        private static void LoadSentences()
        {
            foreach (string type in Enum.GetNames(typeof(SentenceType)))
            {
                if (File.Exists("Config/Sentences/" + type + ".txt"))
                    Sentences.Add(Enum.Parse<SentenceType>(type), File.ReadAllLines("Config/Sentences/" + type + ".txt"));
            }
        }

        public enum SentenceType
        {
            Welcome,
            StaffNewbieOnVoiceChannel,
        }

        public static string GetRandomSentence(SentenceType _type, string _tagToReplace, string _data)
        {
            if (!Sentences.ContainsKey(_type)) return "Error Any Sentence for : " + _type;

            string[] array = Sentences[_type];
            string sentence = array[new Random().Next(array.Length)];
            return sentence.Replace('{' + _tagToReplace + '}', _data);
        }
    }
}
