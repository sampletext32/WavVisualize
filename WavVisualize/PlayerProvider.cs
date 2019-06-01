using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace WavVisualize
{
    class PlayerProvider
    {
        private WindowsMediaPlayer _wmp;

        public float GetNormalizedPosition()
        {
            return GetElapsedSeconds() / GetDurationSeconds();
        }

        public float GetElapsedSeconds()
        {
            return (float) _wmp.controls.currentPosition;
        }

        public float GetDurationSeconds()
        {
            return (float) _wmp.currentMedia.duration;
        }

        public void SetNormalizedPosition(float position)
        {
            _wmp.controls.currentPosition = position * _wmp.currentMedia.duration;
        }

        public void SetFile(string filename)
        {
            _wmp.currentMedia = _wmp.newMedia(filename);
        }

        public void Play()
        {
            _wmp.controls.play();
        }

        public void Pause()
        {
            _wmp.controls.pause();
        }

        public PlayerProvider()
        {
            _wmp = new WindowsMediaPlayer();
        }
    }
}