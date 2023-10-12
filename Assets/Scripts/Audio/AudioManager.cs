using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using WillakeD.CommonPatterns;
using Sirenix.OdinInspector;

namespace Game.Audios
{
    public enum DirectionOptions
    {
        Left,
        Right
    }
    public class AudioManager : Singleton<AudioManager>
    {
        [Title("References")]
        public AudioMixer masterMixerGroup;
        private AudioMixerGroup _musicMixerGroup;
        private AudioMixerGroup _sfxMixerGroup;
        public const string PARAM_NAME_MUSIC_VOLUME = "MusicVolume";
        public const string PARAM_NAME_SFX_VOLUME = "SFXVolume";

        public bool isSfxMuted { get; private set; } = false;
        public bool isMusicMuted { get; private set; } = false;
        public bool isMusicPaused { get; private set; } = false;

        [Title("Settings")]
        public float maxVolume = 0.0f;
        public float minVolume = -60.0f;
        public int sfxSourcePoolSize = 20;
        private AudioSource _musicSource;
        private Queue<AudioSource> _sfxSourcePool;
        private float _cachedSFXVolume = 0.0f;
        private float _cachedMusicVolume = 0.0f;

        public bool IsMusicPlaying { get => _musicSource.isPlaying; }
        public float MusicVolume { get => _cachedMusicVolume; }
        public float MusicVolumePercentage { get => (_cachedMusicVolume + 60.0f) / 60.0f; }
        public float SFXVolume { get => _cachedSFXVolume; }
        public float SFXVolumePercentage { get => (_cachedSFXVolume + 60.0f) / 60.0f; }
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
                GameObject go = new GameObject();
                go.name = $"sfx_{i}";
                go.transform.SetParent(this.transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _sfxMixerGroup;
                source.playOnAwake = false;
                _sfxSourcePool.Enqueue(source);
            }

            masterMixerGroup.SetFloat(PARAM_NAME_MUSIC_VOLUME, maxVolume);
            _cachedMusicVolume = maxVolume;
            masterMixerGroup.SetFloat(PARAM_NAME_SFX_VOLUME, maxVolume);
            _cachedSFXVolume = maxVolume;
        }

        public void SetMusicMuted(bool muted)
        {
            isMusicMuted = muted;

            SetMusicVolume(muted ? minVolume : _cachedMusicVolume);
        }

        public void SetSFXMuted(bool muted)
        {
            isSfxMuted = muted;

            SetSFXVolume(muted ? minVolume : _cachedMusicVolume);
        }

        // 0 ~ -60 db
        public void SetMusicVolume(float volume)
        {
            if (isMusicMuted == false)
            {
                masterMixerGroup.SetFloat(PARAM_NAME_MUSIC_VOLUME, volume);
            }
            _cachedMusicVolume = volume;
        }

        public void SetMusicVolumeByPercentage(float p)
        {
            float percentage = Mathf.Clamp(p, 0.0f, 1.0f);

            float volume = -60 + (60 * percentage);

            masterMixerGroup.SetFloat(PARAM_NAME_MUSIC_VOLUME, volume);

            _cachedMusicVolume = volume;
        }

        // 0 ~ -60 db
        public void SetSFXVolume(float volume)
        {
            if (isSfxMuted == false)
            {
                masterMixerGroup.SetFloat(PARAM_NAME_SFX_VOLUME, volume);
            }
            _cachedSFXVolume = volume;
        }

        public void SetSFXVolumeByPercentage(float p)
        {
            float percentage = Mathf.Clamp(p, 0.0f, 1.0f);

            float volume = -60 + (60 * percentage);

            masterMixerGroup.SetFloat(PARAM_NAME_SFX_VOLUME, volume);

            _cachedSFXVolume = volume;
        }

        public void ResetVolume()
        {
            SetMusicVolume(maxVolume);
            SetSFXVolume(maxVolume);
        }

        public void PlaySFX(AudioClip clip, float volume = 1, float pitch = 1f)
        {
            if (isSfxMuted) return;
            AudioSource source = _sfxSourcePool.Dequeue();
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo = 0f;
            source.PlayOneShot(clip);

            _sfxSourcePool.Enqueue(source);
        }

        public void PlaySFXDirectional(AudioClip clip, DirectionOptions direction, float volume = 1, float pitch = 1f)
        {
            if (isSfxMuted) return;
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
            if (isMusicMuted) return;

            _musicSource.clip = music;
            _musicSource.volume = volume;
            _musicSource.pitch = pitch;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        public void PauseMusic()
        {
            isMusicPaused = true;
            _musicSource.Pause();
        }

        public void UnpuaseMusic()
        {
            isMusicPaused = false;
            _musicSource.UnPause();
        }
    }
}