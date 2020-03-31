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

        private VkHandler(VkApi api)
        {
            _api = api;
        }
    }
}