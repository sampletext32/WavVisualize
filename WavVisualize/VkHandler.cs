using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace WavVisualize
{
    public class VkHandler
    {
        private VkApi _api;

        public static VkHandler Login(string login, string password)
        {
            var services = new ServiceCollection();

            services.AddAudioBypass();

            var api = new VkApi(services);
            api.Authorize(new ApiAuthParams
            {
                Login = login,
                Password = password,
                TwoFactorAuthorization = () =>
                {
                    Console.WriteLine(" > Код двухфакторной аутентификации:");
                    return Console.ReadLine();
                }
            });
            return new VkHandler(api);
        }

        public void GetAudios()
        {
            if (_api.UserId == null)
            {
                Debug.WriteLine("UserID null! in GetAudios");
                return;
            }

            VkCollection<Audio> audios = _api.Audio.Get(new AudioGetParams
                {Count = _api.Audio.GetCount((long) _api.UserId)});
            foreach (var audio in audios)
            {
                Debug.WriteLine($" > {audio.Artist} - {audio.Title} - {audio?.Url}");
            }

            //audios.ToList();
        }

        public string GetFirstAudioUrl()
        {
            if (_api.UserId == null)
            {
                Debug.WriteLine("UserID null! in GetAudios");
                return "";
            }

            VkCollection<Audio> audios = _api.Audio.Get(new AudioGetParams {Count = 1});

            return audios[0].Url.ToString();

            //audios.ToList();
        }

        public static string ConvertM3U8ToMp3Url(string m3u8Url)
        {
            //M3U8 - https://
            //cs1-60v4.vkuseraudio.net/
            //p5/
            //76ce3802735/
            //ec33d12145c3df/
            //index.m3u8
            //?extra=Ik4FLqrrtzGIypVVRwCVLgbX9fT2g34_oCNO-BGfu9apU7tyGra5kUVtKucUWvaXsC0czWqlxUndFq5DCoOOL4RkYKnVErsLy0gxwmn31M4fOWYQ1GyRWFKHULzLxf-I1FIx05WR-KIvcn1MuMiOOPQ

            //MP3 - https://
            //cs1-60v4.vkuseraudio.net/
            //p5/
            //ec33d12145c3df.mp3
            //?extra=Ik4FLqrrtzGIypVVRwCVLgbX9fT2g34_oCNO-BGfu9apU7tyGra5kUVtKucUWvaXsC0czWqlxUndFq5DCoOOL4RkYKnVErsLy0gxwmn31M4fOWYQ1GyRWFKHULzLxf-I1FIx05WR-KIvcn1MuMiOOPQ

            string extra = m3u8Url.Remove(0, m3u8Url.IndexOf("?extra"));

            //all that is not extra
            string mainUrl = m3u8Url.Substring(0, m3u8Url.IndexOf("?extra"));

            //trim https://
            mainUrl = mainUrl.Remove(0, 8);

            string[] tokens = mainUrl.Split('/');

            string url = "https://" + tokens[0] + "/" + tokens[1] + "/" + tokens[3] + ".mp3" + extra;

            return url;
        }

        public static string ExtractMp3NameFromM3U8Url(string m3u8Url)
        {
            //all that is not extra
            string mainUrl = m3u8Url.Substring(0, m3u8Url.IndexOf("?extra"));
            //trim https://
            mainUrl = mainUrl.Remove(0, 8);
            string[] tokens = mainUrl.Split('/');
            string mp3Name = tokens[3];
            return mp3Name;
        }

        public static string CustomAuthorize(string login, string password)
        {
            var webClient = new WebClient();
            webClient.Headers.Clear();
            webClient.Headers["User-Agent"] =
                "VKAndroidApp/5.47.1-4248 (Android 9; SDK 28; armeabi-v7a; Android; ru; 1920x1080)";
            webClient.Headers["x-vk-android-client"] = "new";
            NameValueCollection parameters = new NameValueCollection();
            parameters["grant_type"] = "password";
            parameters["client_id"] = "2274003";
            parameters["client_secret"] = "hHbZxrka2uZ6jB1inYsH";
            parameters["2fa_supported"] = "1";
            parameters["username"] = login;
            parameters["password"] = password;
            parameters["scope"] = "all";
            // ReSharper disable once StringLiteralTypo
            parameters["device_id"] = "Jfq_8zU6w0QJZXVH";
            parameters["v"] = "5.103";
            var result = webClient.UploadString("https://oauth.vk.com/token", "POST",
                string.Join("&", parameters.AllKeys.Select(t => t + "=" + parameters[t])));

            result = result.Remove(0, 17);
            result = result.Substring(0, result.IndexOf("\""));

            return result;
        }

        private VkHandler(VkApi api)
        {
            _api = api;
        }
    }
}