// 役割: スキル発動とクールダウン管理(最大 4 スロット: 3 + Ultimate)。
using System;
using UnityEngine;
using Ironbound.Data;
using Ironbound.Audio;

namespace Ironbound.Combat
{
    public class SkillComponent : MonoBehaviour
    {
        public const int Slots = 4;        // 0..2: Q/C/V, 3: Ultimate
        [SerializeField] private SkillData[] skills = new SkillData[Slots];
        [SerializeField] private float[] cooldowns = new float[Slots];
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private ManaComponent mana;
        [SerializeField] private GameObject projectilePrefabFallback;

        public event Action<int, float, float> OnCooldownChanged; // slot, remaining, total

        public void Configure(SkillData[] slotSkills, ManaComponent manaRef, LayerMask mask)
        {
            for (int i = 0; i < Slots; i++) skills[i] = i < slotSkills.Length ? slotSkills[i] : null;
            mana = manaRef;
            hitMask = mask;
        }

        public SkillData GetSkill(int slot) => (slot < 0 || slot >= Slots) ? null : skills[slot];
        public float CooldownRemaining(int slot) => cooldowns[slot];

        public bool TryCast(int slot, Transform origin, GameObject owner)
        {
            if (slot < 0 || slot >= Slots) return false;
            var s = skills[slot];
            if (s == null || cooldowns[slot] > 0) return false;
            if (mana != null && s.Cost > 0 && !mana.TrySpend(s.Cost)) return false;

            switch (s.Targeting)
            {
                case SkillTargeting.Projectile:
                    SpawnProjectile(origin, s, owner);
                    break;
                case SkillTargeting.Area:
                case SkillTargeting.ConeMelee:
                case SkillTargeting.Forward:
                    MeleeHitbox.Strike(origin, s, owner, hitMask);
                    break;
                case SkillTargeting.Self:
                    var hp = owner.GetComponent<HealthComponent>();
                    if (hp != null) hp.Heal(s.Damage);
                    break;
            }
            cooldowns[slot] = s.Cooldown;
            OnCooldownChanged?.Invoke(slot, cooldowns[slot], s.Cooldown);
            if (s.SfxCue != null) AudioManager.Instance?.PlayCue(s.SfxCue, origin.position);
            return true;
        }

        private void SpawnProjectile(Transform origin, SkillData s, GameObject owner)
        {
            GameObject go = projectilePrefabFallback != null
                ? Instantiate(projectilePrefabFallback)
                : GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Projectile_" + s.SkillId;
            go.transform.localScale = Vector3.one * 0.3f;
            foreach (var c in go.GetComponents<Collider>()) c.isTrigger = true;
            go.transform.position = origin.position + origin.forward * 0.8f + Vector3.up * 1.2f;
            var proj = go.AddComponent<ProjectileComponent>();
            proj.Damage = s.Damage; proj.Element = s.Element; proj.Owner = owner;
            proj.HitMask = hitMask;
            proj.Launch(origin.forward);
        }

        private void Update()
        {
            for (int i = 0; i < Slots; i++)
            {
                if (cooldowns[i] > 0)
                {
                    cooldowns[i] -= Time.deltaTime;
                    if (cooldowns[i] < 0) cooldowns[i] = 0;
                    if (skills[i] != null) OnCooldownChanged?.Invoke(i, cooldowns[i], skills[i].Cooldown);
                }
            }
        }
    }
}
