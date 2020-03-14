using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TrueSpectrumDrawer
    {
        //Done, using EFLA-E Algorithm (http://www.edepot.com/algorithm.html)
        private static void FastLineAlgorithm(DirectBitmap bitmap, int startX, int startY, int endX, int endY,
            int color)
        {
            bool yLonger = false;
            int shortLen = endY - startY;
            int longLen = endX - startX;
            if (Math.Abs(shortLen) > Math.Abs(longLen))
            {
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
                yLonger = true;
            }

            int decInc;
            if (longLen == 0) decInc = 0;
            else decInc = (shortLen << 16) / longLen;

            if (yLonger)
            {
                if (longLen > 0)
                {
                    longLen += startY;
                    for (int j = 0x8000 + (startX << 16); startY <= longLen; ++startY)
                    {
                        bitmap.SetPixel(j >> 16, startY, color);
                        j += decInc;
                    }

                    return;
                }

                longLen += startY;
                for (int j = 0x8000 + (startX << 16); startY >= longLen; --startY)
                {
                    bitmap.SetPixel(j >> 16, startY, color);
                    j -= decInc;
                }

                return;
            }

            if (longLen > 0)
            {
                longLen += startX;
                for (int j = 0x8000 + (startY << 16); startX <= longLen; ++startX)
                {
                    bitmap.SetPixel(startX, j >> 16, color);
                    j += decInc;
                }

                return;
            }

            longLen += startX;
            for (int j = 0x8000 + (startY << 16); startX >= longLen; --startX)
            {
                bitmap.SetPixel(startX, j >> 16, color);
                j -= decInc;
            }
        }

        public static void Draw(DirectBitmap bitmap, float[] frequencies, float frequencyResolution, int useFullCount,
            int baselineY, float constantHeightMultiplier, int width, int height, int color)
        {
            int lastX = 0;
            int lastY = baselineY - (int)(height * frequencies[0]);//this is basically removing very first band zeroing

            for (int i = 1; i < useFullCount; i++)
            {
                int x = (int) (width * ((float) i / useFullCount));

                //float multiplier = (float)(Math.Log(i * frequencyResolution) * 10);

                int h =
                    (int) (height * frequencies[i]);
                var y = baselineY - h;

                FastLineAlgorithm(bitmap, lastX, lastY, x, y, color);

                lastX = x;
                lastY = y;
            }
        }
    }
}