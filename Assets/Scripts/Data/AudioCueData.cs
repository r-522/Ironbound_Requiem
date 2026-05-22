// 役割: 多レイヤ AudioClip と再生パラメータをまとめる Cue。
using UnityEngine;

namespace Ironbound.Data
{
    [CreateAssetMenu(menuName = "Ironbound/AudioCue")]
    public class AudioCueData : ScriptableObject
    {
        public AudioClip[] Clips;
        public AudioClip LowLayer;     // 低域強化
        public AudioClip MetalLayer;   // 金属高域
        [Range(0f, 1.5f)] public float Volume = 1f;
        [Range(0.5f, 1.5f)] public float PitchMin = 0.95f;
        [Range(0.5f, 1.5f)] public float PitchMax = 1.05f;
        public float MaxDistance = 30f;
        public bool Spatial = true;
    }
}
