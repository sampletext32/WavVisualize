using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TrueSpectrumDrawer
    {
        private static void FastLineAlgorithm(DirectBitmap bitmap, int startX, int startY, int endX, int endY,
            int color)
        {
            bool yLonger = false;
            int incrementVal, endVal;
            int shortLen = endY - startY;
            int longLen = endX - startX;
            if (Math.Abs(shortLen) > Math.Abs(longLen))
            {
                int swap = shortLen;
                shortLen = longLen;
                longLen = swap;
                yLonger = true;
            }

            endVal = longLen;
            if (longLen < 0)
            {
                incrementVal = -1;
                longLen = -longLen;
            }
            else
            {
                incrementVal = 1;
            }

            int decInc;
            if (longLen == 0)
            {
                decInc = 0;
            }
            else
            {
                decInc = (shortLen << 16) / longLen;
            }

            int j = 0;
            if (yLonger)
            {
                for (int i = 0; i != endVal; i += incrementVal)
                {
                    bitmap.SetPixel(startX + (j >> 16), startY + i, color);
                    j += decInc;
                }
            }
            else
            {
                for (int i = 0; i != endVal; i += incrementVal)
                {
                    bitmap.SetPixel(startX + i, startY + (j >> 16), color);
                    j += decInc;
                }
            }
        }

        public static void Draw(DirectBitmap bitmap, float[] frequencies, float frequencyResolution, int useFullCount,
            int baselineY, float constantHeightMultiplier, int width, int height, int color)
        {
            int lastX = 0;
            int lastY = baselineY;

            for (int i = 2; i < useFullCount; i++)
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