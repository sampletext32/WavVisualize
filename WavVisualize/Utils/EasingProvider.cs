namespace WavVisualize
{
    public class EasingProvider
    {
        public static float Ease(float start, float end, float easing)
        {
            return start + (end - start) * easing;
        }

        public static void Ease(float[] origin, float[] end, float easing)
        {
            for (int i = 0; i < origin.Length; i++)
            {
                origin[i] = Ease(origin[i], end[i], easing);
            }
        }
    }
}