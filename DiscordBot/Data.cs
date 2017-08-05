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

        public static string Token { get; } = "(YOUR TOKEN)";

        public static SocketGuild Guild { get; set; }

        public static void LoadConfig()
        {
            string content = File.ReadAllText("config.xml");
            XmlDocument document = new XmlDocument {PreserveWhitespace = false};
            document.LoadXml(content.Trim());

            XmlNode configNode = document.ChildNodes.Cast<XmlNode>().FirstOrDefault(_node => _node.Name == "config");

            foreach (XmlNode node in configNode.ChildNodes)
            {
                if(node.NodeType == XmlNodeType.Element)
                    Configuration.Add(node.Name, node.InnerText);
            }
        }
    }
}
