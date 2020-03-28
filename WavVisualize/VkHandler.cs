using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VkNet;
using VkNet.AudioBypassService.Extensions;

namespace WavVisualize
{
    public class VkHandler
    {
        public void Login()
        {
            var services = new ServiceCollection();

            services.AddAudioBypass();

            var api = new VkApi(services);
        }
    }
}
