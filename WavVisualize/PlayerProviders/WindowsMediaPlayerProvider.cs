using System;
using System.Diagnostics;
using WMPLib;

namespace WavVisualize
{
    public class WindowsMediaPlayerProvider
    {
        private WindowsMediaPlayer _player;

        private PlayState _playState;

        public event Action OnPlayEnd;
        public event Action OnPlayStart;

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
                return (float) _player.controls.currentPosition;
            }

            return 0f;
        }

        public float GetDurationSeconds()
        {
            if (_playState != PlayState.NonInitialized)
            {
                return (float) _player.currentMedia.duration;
            }

            return 0f;
        }

        public void SetNormalizedPosition(float position)
        {
            if (_playState != PlayState.NonInitialized)
            {
                _player.controls.currentPosition = position * _player.currentMedia.duration;
            }
        }

        public void SetFile(string filename)
        {
            _player.currentMedia = _player.newMedia(filename);
        }

        public void Play()
        {
            if (_player.playState == WMPPlayState.wmppsUndefined)
            {
                Debug.WriteLine("Play detected without file");
                return;
            }

            _player.controls.play();
            _playState = PlayState.Playing;
        }

        public void Pause()
        {
            _player.controls.pause();
            _playState = PlayState.Paused;
        }

        public WindowsMediaPlayerProvider()
        {
            _playState = PlayState.NonInitialized;
            _player = new WindowsMediaPlayer();
            _player.PlayStateChange += PlayerPlayStateChange;
            _player.settings.autoStart = false;
        }

        private void PlayerPlayStateChange(int newState)
        {
            _playState = _player.playState == WMPPlayState.wmppsPlaying ? PlayState.Playing : PlayState.Paused;
            if (_player.playState == WMPPlayState.wmppsStopped)
            {
                OnPlayEnd?.Invoke();
            }
            else if (_player.playState == WMPPlayState.wmppsPaused)
            {
                OnPlayEnd?.Invoke();
            }
            else if (_player.playState == WMPPlayState.wmppsPlaying)
            {
                OnPlayStart?.Invoke();
            }
        }
    }
}