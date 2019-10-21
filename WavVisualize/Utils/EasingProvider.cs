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
                origin[i] = FallEase(origin[i], end[i], easing);
            }
        }

        public static float FallEase(float start, float end, float easing)
        {
            if(end > start)
            {
                return end;
            }
            else
            {
                return Ease(start, end, easing);
            }
        }
    }
}