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
            VkCollection<Audio> audios = _api.Audio.Get(new AudioGetParams { Count = _api.Audio.GetCount((long) _api.UserId) });
            foreach (var audio in audios)
            {
                Debug.WriteLine($" > {audio.Artist} - {audio.Title} - {audio?.Url}");
            }

            //audios.ToList();
        }

        private VkHandler(VkApi api)
        {
            _api = api;
        }
    }
}