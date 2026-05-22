// 役割: 選択クラスのデータをコンポーネントへ適用。
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.Player
{
    public class ClassLoadout : MonoBehaviour
    {
        public PlayerClassData Class;
        [SerializeField] private HealthComponent hp;
        [SerializeField] private StaminaComponent stamina;
        [SerializeField] private ManaComponent mana;
        [SerializeField] private ComboController combo;
        [SerializeField] private SkillComponent skills;
        [SerializeField] private LayerMask enemyMask;

        private void Reset()
        {
            hp = GetComponent<HealthComponent>();
            stamina = GetComponent<StaminaComponent>();
            mana = GetComponent<ManaComponent>();
            combo = GetComponent<ComboController>();
            skills = GetComponent<SkillComponent>();
        }

        public void Apply(PlayerClassData data)
        {
            Class = data;
            if (data == null) return;
            hp?.Configure(data.MaxHP);
            stamina?.Configure(data.MaxStamina);
            mana?.Configure(data.MaxMana);
            combo?.Configure(data.BasicAttack, enemyMask);
            var slots = new SkillData[4];
            if (data.Skills != null) for (int i = 0; i < Mathf.Min(3, data.Skills.Length); i++) slots[i] = data.Skills[i];
            slots[3] = data.UltimateSkill;
            skills?.Configure(slots, mana, enemyMask);
        }
    }
}
