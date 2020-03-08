using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TrueWaveformProvider
    {
        public enum RecreationMode
        {
            /// <summary>
            /// Very Basic And StraightForward, Single For Loop
            /// </summary>
            Sequential = 0,

            /// <summary>
            /// Very Basic But Draws From N Different Points
            /// <para>Single For Loop Divided Into N Points</para>
            /// <para>Draws With Given degreeOfParallelism</para>
            /// </summary>
            Parallel = 1
        }

        private static void WriteSample(DirectBitmap bitmap, int leftColor, int rightColor, int leftSample,
            int rightSample,
            int x, float baseLineLeft, float baseLineRight)
        {
            if (leftSample < 0)
            {
                for (float y = baseLineLeft; y < baseLineLeft - leftSample; y++)
                {
                    bitmap.SetPixel(x, (int) y, leftColor);
                }
            }
            else
            {
                for (float y = baseLineLeft - leftSample; y < baseLineLeft; y++)
                {
                    bitmap.SetPixel(x, (int) y, leftColor);
                }
            }

            if (rightSample < 0)
            {
                for (float y = baseLineRight; y < baseLineRight - rightSample; y++)
                {
                    bitmap.SetPixel(x, (int) y, rightColor);
                }
            }
            else
            {
                for (float y = baseLineRight - rightSample; y < baseLineRight; y++)
                {
                    bitmap.SetPixel(x, (int) y, rightColor);
                }
            }
        }

        private static void DirectWaveformMapping(int leftColor, int rightColor,
            float[] leftChannel, float[] rightChannel,
            int samplesCount,
            int startSample, int endSample, float verticalScale,
            DirectBitmap directBitmap, int takeRate)
        {
            float verticalHalf = directBitmap.Height / 2f;
            float verticalQuarter = verticalHalf / 2;
            float verticalThreeQuarters = verticalHalf * 3f / 2f;

            float maxVerticalSize = verticalQuarter * verticalScale;

            for (int currentSample = startSample; currentSample < endSample; currentSample += takeRate)
            {
                int xPosition =
                    (int) (currentSample / (float) samplesCount * directBitmap.Width);

                int valueL =
                    (int) (leftChannel[currentSample] * maxVerticalSize);
                int valueR =
                    (int) (rightChannel[currentSample] * maxVerticalSize);

                WriteSample(directBitmap, leftColor, rightColor, valueL, valueR, xPosition, verticalQuarter,
                    verticalThreeQuarters);
            }
        }

        private static void ParallelWaveformMapping(int leftColor, int rightColor,
            float[] leftChannel, float[] rightChannel,
            int samplesCount,
            int startSample, int endSample, float verticalScale,
            DirectBitmap directBitmap, int degreeOfParallelism)
        {
            CancellationToken cancellationToken = new CancellationToken();
            //Needs To Call Basic Algorithm From N Points
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                Task.Factory.StartNew((k) =>
                {
                    int portion = (int) k;
                    DirectWaveformMapping(leftColor, rightColor, leftChannel, rightChannel,
                        samplesCount, startSample + portion * (endSample - startSample) / degreeOfParallelism,
                        startSample + (portion + 1) * (endSample - startSample) / degreeOfParallelism - 1,
                        verticalScale, directBitmap, 1);
                }, i, cancellationToken);
            }
        }

        public void Recreate(Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey("leftColor") ||
                !parameters.ContainsKey("rightColor") ||
                !parameters.ContainsKey("leftChannel") ||
                !parameters.ContainsKey("rightChannel") ||
                !parameters.ContainsKey("samplesCount") ||
                !parameters.ContainsKey("verticalScale") ||
                !parameters.ContainsKey("directBitmap") ||
                !parameters.ContainsKey("takeRate"))
            {
                throw new ArgumentException("One Of Required Parameters Missing");
            }

            RecreationMode mode = (RecreationMode) (int) parameters["mode"];

            int leftColor = (int) parameters["leftColor"];
            int rightColor = (int) parameters["rightColor"];
            float[] leftChannel = (float[]) parameters["leftChannel"];
            float[] rightChannel = (float[]) parameters["rightChannel"];
            int samplesCount = (int) parameters["samplesCount"];
            float verticalScale = (float) parameters["verticalScale"];
            DirectBitmap directBitmap = (DirectBitmap) parameters["directBitmap"]; 
            int takeRate = (int)parameters["takeRate"];

            switch (mode)
            {
                case RecreationMode.Sequential:
                    DirectWaveformMapping(leftColor, rightColor, leftChannel, rightChannel,
                        samplesCount, 0, samplesCount, verticalScale, directBitmap, takeRate);
                    break;
                case RecreationMode.Parallel:
                    if (!parameters.ContainsKey("degreeOfParallelism"))
                    {
                        throw new ArgumentException("degreeOfParallelism Missing For This Recreation Mode");
                    }

                    int degreeOfParallelism = (int) parameters["degreeOfParallelism"];

                    ParallelWaveformMapping(leftColor, rightColor, leftChannel, rightChannel,
                        samplesCount, 0, samplesCount, verticalScale, directBitmap, degreeOfParallelism);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void RecreateAsync(Dictionary<string, object> parameters)
        {
            Task.Run(() => Recreate(parameters));
        }
    }
}