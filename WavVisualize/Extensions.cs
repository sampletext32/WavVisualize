using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public static class Extensions
    {
        public static WebClient WithProxy(this WebClient client, string proxy)
        {
            if (proxy != string.Empty)
            {
                string proxyIp = proxy.Split('@')[0].Split(':')[0];
                int proxyPort = int.Parse(proxy.Split('@')[0].Split(':')[1]);
                string proxyLogin = proxy.Split('@')[1].Split(':')[0];
                string proxyPassword = proxy.Split('@')[1].Split(':')[1];

                try
                {
                    client.Proxy = new WebProxy(proxyIp, proxyPort);
                    client.Proxy.Credentials = new NetworkCredential(proxyLogin, proxyPassword);
                }
                catch { }
            }
            return client;
        }

        public static WebClient WithBaseUrl(this WebClient client, string url)
        {
            client.BaseAddress = url;
            return client;
        }
    }
}
