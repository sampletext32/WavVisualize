using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WavVisualize
{
    class VkApi
    {
        private string _userid;
        private string _accesstoken;
        private string _proxy;

        public VkApi(string userId, string accessToken, string proxy = "")
        {
            _userid = userId;
            _accesstoken = accessToken;
            _proxy = proxy;
        }

        public async Task<XmlDocument> ExecuteCommand(string command, NameValueCollection parameters)
        {
            XmlDocument result = new XmlDocument();
            WebClient wc = new WebClient().WithProxy(_proxy);
            //System.Threading.Thread.Sleep(1000);
            //LOG.Enqueue("Загрузка");
            byte[] buffer = await wc.UploadValuesTaskAsync(
                string.Format("https://api.vk.com/method/{0}.xml?access_token={1}", command, _accesstoken), parameters);
            result.Load(new MemoryStream(buffer));
            if (CheckCaptcha(result))
            {
                string captcha_sid = result["error"]["captcha_sid"].InnerText;
                string captcha_img_url = result["error"]["captcha_img"].InnerText;
                string captcha_result = await GetCaptchaText(captcha_img_url);
                parameters["captcha_sid"] = captcha_sid;
                parameters["captcha_key"] = captcha_result;
                if (captcha_result == "")
                {
                    return new XmlDocument();
                }
                else
                {
                    buffer = await wc.UploadValuesTaskAsync(
                        string.Format("https://api.vk.com/method/{0}.xml?access_token={1}", command, _accesstoken),
                        parameters);
                    result.Load(new MemoryStream(buffer));
                }
            }

            wc.Dispose();
            wc = null;
            return result;
        }

        private async Task<string> GetCaptchaText(string captchaImgUrl)
        {
            return "";
            throw new NotImplementedException();
        }
        private bool CheckCaptcha(XmlDocument doc)
        {
            if (doc["response"] == null)
            {
                if (doc["error"]["error_msg"].InnerText == "Captcha needed")
                {
                    return true;
                }
            }
            return false;
        }
    }
}