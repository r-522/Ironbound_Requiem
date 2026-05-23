// 役割: AudioCueData を再生する軽量プール。低域/金属レイヤを重ねる。
using System.Collections.Generic;
using UnityEngine;
using Ironbound.Data;

namespace Ironbound.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        [Range(0f, 1f)] public float MasterVolume = 1f;
        [Range(0f, 1f)] public float SfxVolume = 1f;
        [Range(0f, 1f)] public float BgmVolume = 0.7f;

        private readonly Queue<AudioSource> _pool = new();
        private AudioSource _bgm;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            for (int i = 0; i < 12; i++) _pool.Enqueue(CreateSource());
            _bgm = CreateSource(); _bgm.loop = true; _bgm.spatialBlend = 0f;
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject("AudioSrc");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.rolloffMode = AudioRolloffMode.Logarithmic;
            return src;
        }

        public void PlayCue(AudioCueData cue, Vector3 position)
        {
            if (cue == null || cue.Clips == null || cue.Clips.Length == 0) return;
            Play(cue, position, cue.Clips[Random.Range(0, cue.Clips.Length)], 1f);
            if (cue.LowLayer != null) Play(cue, position, cue.LowLayer, 0.85f);
            if (cue.MetalLayer != null) Play(cue, position, cue.MetalLayer, 0.7f);
        }

        private void Play(AudioCueData cue, Vector3 position, AudioClip clip, float vol)
        {
            var src = _pool.Count > 0 ? _pool.Dequeue() : CreateSource();
            src.transform.position = position;
            src.spatialBlend = cue.Spatial ? 1f : 0f;
            src.maxDistance = cue.MaxDistance;
            src.minDistance = 1.5f;
            src.pitch = Random.Range(cue.PitchMin, cue.PitchMax);
            src.volume = cue.Volume * vol * SfxVolume * MasterVolume;
            src.clip = clip; src.Play();
            StartCoroutine(Recycle(src, clip.length / Mathf.Max(0.1f, src.pitch) + 0.05f));
        }

        private System.Collections.IEnumerator Recycle(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay);
            src.Stop(); src.clip = null;
            _pool.Enqueue(src);
        }

        public void PlayBgm(AudioClip clip)
        {
            if (_bgm.clip == clip) return;
            _bgm.clip = clip;
            _bgm.volume = BgmVolume * MasterVolume;
            if (clip != null) _bgm.Play(); else _bgm.Stop();
        }
    }
}
