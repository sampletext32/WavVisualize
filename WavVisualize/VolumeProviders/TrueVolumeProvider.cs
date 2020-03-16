using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TrueVolumeProvider
    {
        public float LastLeft;
        public float LastRight;

        private void MaxInRegionProvider(float[] leftChannel,
            float[] rightChannel,
            int startSample, int useSamples)
        {
            float left = 0f;
            float right = 0f;

            for (int i = startSample; i < startSample + useSamples; i++)
            {
                if (Math.Abs(leftChannel[i]) > left)
                {
                    left = Math.Abs(leftChannel[i]);
                }

                if (Math.Abs(rightChannel[i]) > right)
                {
                    right = Math.Abs(rightChannel[i]);
                }
            }

            LastLeft = left;
            LastRight = right;
        }

        private void AverageInRegionProvider(float[] leftChannel,
            float[] rightChannel,
            int startSample, int useSamples)
        {
            float left = 0f;
            float right = 0f;

            for (int i = startSample; i < startSample + useSamples; i++)
            {
                left += Math.Abs(leftChannel[i]);
                right += Math.Abs(rightChannel[i]);
            }

            //нормализуем обе громкости
            left /= useSamples;
            right /= useSamples;

            LastLeft = left;
            LastRight = right;
        }

        public void Recreate(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey("leftChannel") ||
                !parameters.ContainsKey("rightChannel") ||
                !parameters.ContainsKey("startSample") ||
                !parameters.ContainsKey("useSamples") ||
                !parameters.ContainsKey("type"))
            {
                throw new ArgumentException("One Of Required Parameters Missing");
            }

            float[] leftChannel = (float[]) parameters["leftChannel"];
            float[] rightChannel = (float[]) parameters["rightChannel"];
            int startSample = (int) parameters["startSample"];
            int useSamples = (int) parameters["useSamples"];
            int type = (int) parameters["type"];

            //TODO: Make Type An Enum
            if (type == 1)
            {
                MaxInRegionProvider(leftChannel, rightChannel, startSample, useSamples);
            }
            else if (type == 2)
            {
                AverageInRegionProvider(leftChannel, rightChannel, startSample, useSamples);
            }
            else
            {
                throw new InvalidOperationException("Type is incorrect: " + type);
            }
        }

        public void RecreateAsync(Dictionary<string, object> parameters)
        {
            Task.Run(() => Recreate(parameters));
        }
    }
}