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
using Newtonsoft.Json.Linq;

namespace WavVisualize
{
    class VkApi
    {
        private string _userid;
        private string _accesstoken;
        private string _proxy;

        private VkApi(string userId, string accessToken, string proxy = "")
        {
            _userid = userId;
            _accesstoken = accessToken;
            _proxy = proxy;
        }

        public static async Task<VkApi> Authorize(string login, string password)
        {
            NameValueCollection parameters = new NameValueCollection()
            {
                {"grant_type", "password"},
                {"client_id", "2274003"},
                {"client_secret", "hHbZxrka2uZ6jB1inYsH"},
                //default TwoFactorSupported to true
                //{"2fa_supported", _apiAuthParams.TwoFactorSupported ?? true},
                {"2fa_supported", "1"},
                //default ForceSms to false
                //{"force_sms", _apiAuthParams.ForceSms},
                {"force_sms", "0"},

                //TODO: Set login and password
                {"username", login},
                {"password", password},

                //IMPORTANT: CODE IS CAPTCHA RESOLVED STRING!!!
                //default Code to 0
                //{"code", _apiAuthParams.Code},
                {"code", "0"},
                {"scope", "all"},

                //TODO: Save generated string
                {"device_id", RandomString.Generate(16)}
            };
            WebClient wc = new WebClient().WithBaseUrl("https://oauth.vk.com/token");
            byte[] buffer = await wc.UploadValuesTaskAsync("", parameters);

            var jObject = JObject.Parse(Encoding.UTF8.GetString(buffer));

            return new VkApi(jObject["user_id"].ToString(), jObject["access_token"].ToString());
        }

        public async Task<JToken> ExecuteCommand(string method, NameValueCollection parameters)
        {
            WebClient wc = new WebClient().WithProxy(_proxy).WithBaseUrl("https://api.vk.com/method/");

            parameters["access_token"] = _accesstoken;
            parameters["v"] = "5.103";

            byte[] buffer = await wc.UploadValuesTaskAsync(method, parameters);

            var jObject = JObject.Parse(Encoding.UTF8.GetString(buffer))["response"];

            await EnsureNoCaptcha(jObject);

            wc.Dispose();
            return jObject;
        }

        private static async Task EnsureNoCaptcha(JToken obj)
        {
            if (CheckCaptcha(obj))
            {
                throw new ArgumentException("Captcha detected");
            }

            // Captcha solve


            //string captcha_result = await GetCaptchaText(jObject["error"]["captcha_img"].ToString());
            //parameters["captcha_sid"] = jObject["error"]["captcha_sid"].ToString();
            //parameters["captcha_key"] = captcha_result;
            //if (captcha_result == "")
            //{
            //    return new JObject();
            //}
            //else
            //{
            //    jObject = await ExecuteCommand(method, parameters);
            //}
        }

        private async Task<string> GetCaptchaText(string captchaImgUrl)
        {
            return "";
            throw new NotImplementedException();
        }

        private static bool CheckCaptcha(JToken obj)
        {
            return obj == null && obj["error"]["error_msg"].ToString() == "Captcha needed";
        }
    }
}