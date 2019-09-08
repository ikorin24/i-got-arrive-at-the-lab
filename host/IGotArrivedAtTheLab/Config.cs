using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IGotArrivedAtTheLab
{
    [XmlRoot]
    public class Config
    {
        private const string FILE = "config.xml";

        public string SlackToken { get; set; }
        public string Channel { get; set; }
        public string TestChannel { get; set; }
        public int Baudrate { get; } = 19200;
        public string MsgHeader { get; set; } = "MSG";
        public string Ping { get; } = "Ping";
        public string ConnectedMsg { get; } = "CNCT";
        public string DisconnectedMsg { get; } = "DSCN";
        public string ArrivingMsg { get; } = "I got arrived";
        public string TestArrivingMsg { get; } = "test         ";
        public string OKMsg { get; } = "OK";

        public static Config Instance { get; private set; }

        public static void Load()
        {
            var reader = new XmlSerializer(typeof(Config));
            using(var stream = File.OpenRead(FILE)) {
                Instance = reader.Deserialize(stream) as Config;
            }
        }
    }
}
