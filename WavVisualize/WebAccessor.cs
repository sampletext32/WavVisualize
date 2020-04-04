using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class WebAccessor
    {
        public static byte[] LoadBytes(string url)
        {
            return new WebClient().DownloadData(url);
        }
        public static string LoadString(string url)
        {
            return new WebClient().DownloadString(url);
        }
    }
}