using System;

namespace IGotArrivedAtTheLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Load();

            var comm = new Comm();
            try {
                var slack = new Slack();
                comm.Start();
                comm.MessageReceved += (message) =>
                {
                    if(message == Config.Instance.ArrivingMsg) {
                        comm.SendMessage(Config.Instance.OKMsg);
                        slack.Send(GetSlackText(), Config.Instance.Channel);
                    }
                    else if(message == Config.Instance.TestArrivingMsg) {
                        comm.SendMessage(Config.Instance.OKMsg);
                        slack.Send(GetSlackText() + "(test)", Config.Instance.TestChannel);
                    }
                };

                string input = null;
                while(true) {
                    Console.WriteLine("Press 'q' to quit: ");
                    input = Console.ReadLine();
                    if(input == "q") {
                        break;
                    }
                }
            }
            finally {
                comm.SendMessage(Config.Instance.DisconnectedMsg);
                comm.Dispose();
            }
        }

        static string[] _color = new[] { ":lab_blue:", ":lab_orange:", ":lab_red:", ":lab_green:" };
        static string[] _in = new[] { ":in_water:", ":in_purple:" };
        static Random _rand = new Random();

        private static string GetSlackText()
        {
            var c = _color[_rand.Next(_color.Length)];
            var i = _in[_rand.Next(_in.Length)];
            return $"{c}{i}しました!";
        }
    }
}
