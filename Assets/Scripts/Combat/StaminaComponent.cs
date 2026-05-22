// 役割: スタミナ管理(回避や強攻撃のコスト)。
using System;
using UnityEngine;

namespace Ironbound.Combat
{
    public class StaminaComponent : MonoBehaviour
    {
        [SerializeField] private float max = 100f;
        [SerializeField] private float current = 100f;
        [SerializeField] private float regenPerSec = 18f;
        [SerializeField] private float regenDelay = 0.6f;
        private float _delayTimer;

        public float Max => max;
        public float Current => current;
        public float Normalized => max > 0 ? current / max : 0;
        public event Action<float, float> OnChanged;

        public void Configure(float maxV) { max = maxV; current = maxV; OnChanged?.Invoke(current, max); }

        public bool TrySpend(float cost)
        {
            if (current < cost) return false;
            current -= cost;
            _delayTimer = regenDelay;
            OnChanged?.Invoke(current, max);
            return true;
        }

        private void Update()
        {
            if (_delayTimer > 0) { _delayTimer -= Time.deltaTime; return; }
            if (current < max)
            {
                current = Mathf.Min(max, current + regenPerSec * Time.deltaTime);
                OnChanged?.Invoke(current, max);
            }
        }
    }
}
