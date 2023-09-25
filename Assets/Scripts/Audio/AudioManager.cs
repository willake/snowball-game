using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using WillakeD.CommonPatterns;

namespace Game.Audios
{
    public enum DirectionOptions
    {
        Left,
        Right
    }
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("References")]
        public AudioMixer masterMixerGroup;
        private AudioMixerGroup _musicMixerGroup;
        private AudioMixerGroup _sfxMixerGroup;
        public const string PARAM_NAME_MUSIC_VOLUME = "MusicVolume";
        public const string PARAM_NAME_SFX_VOLUME = "SFXVolume";

        public bool IsSfxMuted { get; private set; } = false;
        public bool IsMusicMuted { get; private set; } = false;
        public bool IsMusicPaused { get; private set; } = false;

        [Header("Settings")]
        public float maxVolume = 0.0f;
        public float minVolume = -60.0f;
        public int sfxSourcePoolSize = 20;
        private AudioSource _musicSource;
        private Queue<AudioSource> _sfxSourcePool;
        private float _cachedSfxVolume = 0.0f;
        private float _cachedMusicVolume = 0.0f;

        private void Start()
        {
            _musicMixerGroup = masterMixerGroup.FindMatchingGroups("Music")[0];
            _sfxMixerGroup = masterMixerGroup.FindMatchingGroups("SFX")[0];

            GameObject musicGo = new GameObject();
            musicGo.name = "music";
            musicGo.transform.SetParent(this.transform);
            _musicSource = musicGo.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _musicMixerGroup;
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

            _sfxSourcePool = new Queue<AudioSource>();

            for (int i = 0; i < sfxSourcePoolSize; i++)
            {
                GameObject go = new GameObject
                {
                    name = $"sfx_{i}"
                };
                go.transform.SetParent(this.transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _sfxMixerGroup;
                source.playOnAwake = false;
                _sfxSourcePool.Enqueue(source);
            }

            masterMixerGroup.SetFloat(PARAM_NAME_MUSIC_VOLUME, maxVolume);
            _cachedMusicVolume = maxVolume;
            masterMixerGroup.SetFloat(PARAM_NAME_SFX_VOLUME, maxVolume);
            _cachedSfxVolume = maxVolume;
        }

        public void SetMusicMuted(bool muted)
        {
            IsMusicMuted = muted;

            SetMusicVolume(muted ? minVolume : _cachedMusicVolume);
        }

        public void SetSfxMuted(bool muted)
        {
            IsSfxMuted = muted;

            SetSfxVolume(muted ? minVolume : _cachedSfxVolume);
        }

        // 0 ~ -60 db
        public void SetMusicVolume(float volume)
        {
            if (IsMusicMuted == false)
            {
                masterMixerGroup.SetFloat(PARAM_NAME_MUSIC_VOLUME, volume);
            }
            _cachedMusicVolume = volume;
        }

        // 0 ~ -60 db
        public void SetSfxVolume(float volume)
        {
            if (IsSfxMuted == false)
            {
                masterMixerGroup.SetFloat(PARAM_NAME_SFX_VOLUME, volume);
            }
            _cachedSfxVolume = volume;
        }

        public void ResetVolume()
        {
            SetMusicVolume(maxVolume);
            SetSfxVolume(maxVolume);
        }

        public void PlaySfx(AudioClip clip, float volume = 1, float pitch = 1f)
        {
            if (IsSfxMuted) return;
            AudioSource source = _sfxSourcePool.Dequeue();
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo = 0f;
            source.PlayOneShot(clip);

            _sfxSourcePool.Enqueue(source);
        }

        public void PlaySfxDirectional(AudioClip clip, DirectionOptions direction, float volume = 1, float pitch = 1f)
        {
            if (IsSfxMuted) return;
            AudioSource source = _sfxSourcePool.Dequeue();
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo =
                direction == DirectionOptions.Left ? -0.75f : 0.75f;
            source.PlayOneShot(clip);

            _sfxSourcePool.Enqueue(source);
        }

        public void PlayMusic(AudioClip music, float volume = 1, float pitch = 1f)
        {
            if (IsSfxMuted) return;

            _musicSource.clip = music;
            _musicSource.volume = volume;
            _musicSource.pitch = pitch;
            _musicSource.Play();
        }

        public void PauseMusic()
        {
            IsMusicPaused = true;
            _musicSource.Pause();
        }

        public void UnpauseMusic()
        {
            IsMusicPaused = false;
            _musicSource.UnPause();
        }
    }
}