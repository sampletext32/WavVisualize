using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace WavVisualize
{
    public class PlayerProvider
    {
        private WindowsMediaPlayer _wmp;

        private PlayState _playState;

        public bool IsPlaying()
        {
            return _playState == PlayState.Playing;
        }

        public float GetNormalizedPosition()
        {
            if (_playState != PlayState.NonInitialized)
            {
                return GetElapsedSeconds() / GetDurationSeconds();
            }

            return 0f;
        }

        public float GetElapsedSeconds()
        {
            if (_playState != PlayState.NonInitialized)
            {
                return (float) _wmp.controls.currentPosition;
            }

            return 0f;
        }

        public float GetDurationSeconds()
        {
            if (_playState != PlayState.NonInitialized)
            {
                return (float) _wmp.currentMedia.duration;
            }

            return 0f;
        }

        public void SetNormalizedPosition(float position)
        {
            if (_playState != PlayState.NonInitialized)
            {
                _wmp.controls.currentPosition = position * _wmp.currentMedia.duration;
            }
        }

        public void SetFile(string filename)
        {
            _wmp.currentMedia = _wmp.newMedia(filename);
        }

        public void Play()
        {
            _wmp.controls.play();
            _playState = PlayState.Playing;
        }

        public void Pause()
        {
            _wmp.controls.pause();
            _playState = PlayState.Paused;
        }

        public PlayerProvider()
        {
            _playState = PlayState.NonInitialized;
            _wmp = new WindowsMediaPlayer();
        }
    }
}