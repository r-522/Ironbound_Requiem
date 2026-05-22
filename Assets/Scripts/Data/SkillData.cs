// 役割: スキルおよび通常/強攻撃のデータ定義。
using UnityEngine;

namespace Ironbound.Data
{
    public enum DamageElement { Physical, Fire, Frost, Lightning, Arcane, Holy, Shadow }
    public enum SkillTargeting { Self, Forward, Area, Projectile, ConeMelee }

    [CreateAssetMenu(menuName = "Ironbound/Skill")]
    public class SkillData : ScriptableObject
    {
        public string SkillId;
        public string DisplayName;
        [TextArea] public string Description;
        public float Cooldown = 4f;
        public float Cost = 0f;
        public float Damage = 25f;
        public float Range = 2f;
        public float Radius = 1.5f;
        public float CastTime = 0.15f;
        public float RecoveryTime = 0.25f;
        public float Knockback = 2f;
        public float HitStopMs = 60f;
        public float CameraShakeAmp = 0.15f;
        public DamageElement Element = DamageElement.Physical;
        public SkillTargeting Targeting = SkillTargeting.ConeMelee;
        public GameObject VfxPrefab;
        public AudioCueData SfxCue;
    }
}
