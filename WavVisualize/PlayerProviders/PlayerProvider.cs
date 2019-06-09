using System;
using WMPLib;

namespace WavVisualize
{
    public class PlayerProvider
    {
        private WindowsMediaPlayer _wmp;

        private PlayState _playState;

        public PlayState GetPlayState()
        {
            return _playState;
        }

        public bool IsPlaying()
        {
            return _playState == PlayState.Playing;
        }

        public bool IsPaused()
        {
            return _playState == PlayState.Paused;
        }

        public float GetNormalizedPosition()
        {
            if (_playState != PlayState.NonInitialized)
            {
                if (Math.Abs(GetDurationSeconds()) < 0.00000001f)
                {
                    return 0f;
                }
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
            _wmp.PlayStateChange += _wmp_PlayStateChange;
            _wmp.settings.autoStart = false;
        }

        private void _wmp_PlayStateChange(int NewState)
        {
            if (_wmp.playState == WMPPlayState.wmppsPlaying)
            {
                _playState = PlayState.Playing;
            }
            else
            {
                _playState = PlayState.Paused;
            }
        }
    }
}