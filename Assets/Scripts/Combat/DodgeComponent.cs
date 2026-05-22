// 役割: 回避ロール。無敵時間と移動を管理。
using System.Collections;
using UnityEngine;

namespace Ironbound.Combat
{
    public class DodgeComponent : MonoBehaviour
    {
        [SerializeField] private float distance = 4.2f;
        [SerializeField] private float duration = 0.32f;
        [SerializeField] private float iFrameStart = 0.05f;
        [SerializeField] private float iFrameEnd = 0.28f;
        public bool IsInvulnerable { get; private set; }
        public bool IsDodging { get; private set; }

        public IEnumerator Roll(CharacterController cc, Vector3 dir, StaminaComponent stamina, float cost)
        {
            if (IsDodging) yield break;
            if (stamina != null && !stamina.TrySpend(cost)) yield break;
            IsDodging = true;
            float t = 0;
            dir = dir.sqrMagnitude < 0.01f ? transform.forward : dir.normalized;
            while (t < duration)
            {
                IsInvulnerable = (t >= iFrameStart && t <= iFrameEnd);
                float v = (distance / duration);
                if (cc != null && cc.enabled) cc.Move(dir * v * Time.deltaTime);
                else transform.position += dir * v * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
            IsInvulnerable = false;
            IsDodging = false;
        }
    }
}
