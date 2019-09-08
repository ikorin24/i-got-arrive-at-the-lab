using Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IGotArrivedAtTheLab
{
    public class Slack
    {
        public bool Send(string text, string channel)
        {
            const string url = "https://slack.com/api/chat.postMessage";
            using(var wc = new WebClient()) {
                wc.Headers.Add("Authorization", $"Bearer {Config.Instance.SlackToken}");
                var ps = new NameValueCollection();
                ps.Add("channel", channel);
                ps.Add("text", text);
                ps.Add("as_user", "True");
                var response = wc.UploadValues(url, ps);
                var dic = JsonParser.FromJson(Encoding.UTF8.GetString(response));
                return (bool)dic["ok"];
            }
        }
    }
}
