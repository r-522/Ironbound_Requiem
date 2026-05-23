// 役割: マナ(クラスリソース)管理。
using System;
using UnityEngine;

namespace Ironbound.Combat
{
    public class ManaComponent : MonoBehaviour
    {
        [SerializeField] private float max = 100f;
        [SerializeField] private float current = 100f;
        [SerializeField] private float regenPerSec = 6f;
        public float Max => max;
        public float Current => current;
        public float Normalized => max > 0 ? current / max : 0;
        public event Action<float, float> OnChanged;

        public void Configure(float maxV) { max = maxV; current = maxV; OnChanged?.Invoke(current, max); }
        public bool TrySpend(float cost)
        {
            if (current < cost) return false;
            current -= cost; OnChanged?.Invoke(current, max); return true;
        }
        private void Update()
        {
            if (current < max)
            {
                current = Mathf.Min(max, current + regenPerSec * Time.deltaTime);
                OnChanged?.Invoke(current, max);
            }
        }
    }
}
