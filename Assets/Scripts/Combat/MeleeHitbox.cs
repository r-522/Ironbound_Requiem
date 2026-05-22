// 役割: コーン状の近接判定を一度だけ走らせ、命中対象にダメージ・ノックバック・ヒットストップを適用。
using System.Collections.Generic;
using UnityEngine;
using Ironbound.Core;
using Ironbound.Data;
using Ironbound.Audio;

namespace Ironbound.Combat
{
    public static class MeleeHitbox
    {
        private static readonly Collider[] _buffer = new Collider[24];

        public static int Strike(Transform origin, SkillData skill, GameObject sourceGO, LayerMask hitMask, float armorBonus = 0f)
        {
            if (skill == null) return 0;
            int n = Physics.OverlapSphereNonAlloc(origin.position + origin.forward * (skill.Range * 0.5f),
                                                  skill.Range, _buffer, hitMask, QueryTriggerInteraction.Ignore);
            int hits = 0;
            HashSet<HealthComponent> seen = new();
            for (int i = 0; i < n; i++)
            {
                var col = _buffer[i];
                if (col == null || col.gameObject == sourceGO) continue;
                Vector3 to = col.transform.position - origin.position;
                if (Vector3.Dot(origin.forward, to.normalized) < 0.2f) continue;  // 約 ±80°
                var hp = col.GetComponentInParent<HealthComponent>();
                if (hp == null || seen.Contains(hp)) continue;
                seen.Add(hp);
                float dmg = DamageComponent.Compute(skill.Damage, skill.Element, armorBonus, DamageElement.Physical);
                hp.ApplyDamage(dmg, sourceGO);
                var kb = hp.GetComponent<KnockbackComponent>();
                if (kb != null) kb.Apply(to.normalized, skill.Knockback);
                var react = hp.GetComponent<HitReactionComponent>();
                if (react != null) react.Trigger(to.normalized, 0.15f);
                EventBus.Publish(new DamageDealtEvent { Source = sourceGO, Target = hp.gameObject, Amount = dmg });
                hits++;
            }
            if (hits > 0)
            {
                if (HitStopService.Instance != null) HitStopService.Instance.Freeze(skill.HitStopMs);
                CameraShakeController.Shake(skill.CameraShakeAmp, 0.12f);
                if (skill.SfxCue != null) AudioManager.Instance?.PlayCue(skill.SfxCue, origin.position);
            }
            return hits;
        }
    }
}
