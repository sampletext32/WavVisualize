using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TrueVolumeDrawer
    {
        private static void MasterVolumeMapping(int leftColor, int rightColor, float leftVolumeNormalized,
            float rightVolumeNormalized, int bandWidth, int height, int baselineY, DirectBitmap directBitmap)
        {
            int heightL = (int) (height * (leftVolumeNormalized));
            int heightR = (int) (height * (rightVolumeNormalized));

            for (int i = 0; i < bandWidth; i++)
            {
                for (int j = baselineY - heightL; j <= baselineY; j++)
                {
                    directBitmap.SetPixel(i, j, leftColor);
                }
            }
            for (int i = bandWidth; i < 2 * bandWidth; i++)
            {
                for (int j = baselineY - heightR; j <= baselineY; j++)
                {
                    directBitmap.SetPixel(i, j, rightColor);
                }
            }
        }

        public static void Recreate(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey("leftColor") ||
                !parameters.ContainsKey("rightColor") ||
                !parameters.ContainsKey("leftVolumeNormalized") ||
                !parameters.ContainsKey("rightVolumeNormalized") ||
                !parameters.ContainsKey("bandWidth") ||
                !parameters.ContainsKey("height") ||
                !parameters.ContainsKey("baselineY") ||
                !parameters.ContainsKey("directBitmap"))
            {
                throw new ArgumentException("One Of Required Parameters Missing");
            }

            int leftColor = (int) parameters["leftColor"];
            int rightColor = (int) parameters["rightColor"];
            float leftVolumeNormalized = (float) parameters["leftVolumeNormalized"];
            float rightVolumeNormalized = (float) parameters["rightVolumeNormalized"];
            int bandWidth = (int) parameters["bandWidth"];
            int height = (int) parameters["height"];
            int baselineY = (int) parameters["baselineY"];
            DirectBitmap directBitmap = (DirectBitmap) parameters["directBitmap"];

            MasterVolumeMapping(leftColor, rightColor, leftVolumeNormalized, rightVolumeNormalized, bandWidth, height,
                baselineY, directBitmap);
        }


        public static void RecreateAsync(Dictionary<string, object> parameters)
        {
            Task.Run(() => Recreate(parameters));
        }
    }
}