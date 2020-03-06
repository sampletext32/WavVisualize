namespace WavVisualize
{
    public class PlayerSettings
    {
        //сколько раз в секунду обновляется состояние плеера
        public int UpdateRate { get; }

        //сколько сэмплов идёт на преобразование спектра (обязательно степень двойки)
        public int SpectrumUseSamples { get; }

        //максимальная отрисовываемая частота
        public int TrimFrequency { get; }

        //коэффициент смягчения резких скачков
        public float EasingCoef { get; }

        //применять прореживание по времени 
        public bool ApplyTimeThinning { get; }

        public PlayerSettings(int updateRate, int spectrumUseSamples, int trimFrequency, float easingCoef, bool applyTimeThinning)
        {
            UpdateRate = updateRate;
            SpectrumUseSamples = spectrumUseSamples;
            TrimFrequency = trimFrequency;
            EasingCoef = easingCoef;
            ApplyTimeThinning = applyTimeThinning;
        }
    }
}