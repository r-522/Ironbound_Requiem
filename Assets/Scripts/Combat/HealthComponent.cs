// 役割: HP 管理。被ダメ・回復・死亡判定。
using System;
using UnityEngine;

namespace Ironbound.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float currentHP = 100f;
        public float MaxHP => maxHP;
        public float Current => currentHP;
        public float Normalized => maxHP > 0 ? currentHP / maxHP : 0f;
        public bool IsDead => currentHP <= 0f;

        public event Action<float, float> OnChanged;   // current, max
        public event Action<GameObject> OnDied;        // killer

        public void Configure(float max)
        {
            maxHP = max;
            currentHP = max;
            OnChanged?.Invoke(currentHP, maxHP);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            currentHP = Mathf.Min(maxHP, currentHP + amount);
            OnChanged?.Invoke(currentHP, maxHP);
        }

        public void ApplyDamage(float amount, GameObject source)
        {
            if (IsDead || amount <= 0) return;
            currentHP = Mathf.Max(0, currentHP - amount);
            OnChanged?.Invoke(currentHP, maxHP);
            if (currentHP <= 0) OnDied?.Invoke(source);
        }
    }
}
