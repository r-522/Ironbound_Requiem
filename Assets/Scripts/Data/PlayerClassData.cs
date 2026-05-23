// 役割: プレイヤークラス定義 ScriptableObject。
using UnityEngine;

namespace Ironbound.Data
{
    [CreateAssetMenu(menuName = "Ironbound/PlayerClass")]
    public class PlayerClassData : ScriptableObject
    {
        public string ClassId;
        public string DisplayName;
        [TextArea] public string Description;
        public float MaxHP = 100f;
        public float MaxStamina = 100f;
        public float MaxMana = 100f;
        public float MoveSpeed = 5.5f;
        public float DodgeCost = 25f;
        public SkillData BasicAttack;
        public SkillData HeavyAttack;
        public SkillData[] Skills;       // size = 3
        public SkillData UltimateSkill;
        public string[] PreferredTowers;
        public string[] StartingEquipment;
        public Color ThemeColor = new Color(0.65f, 0.55f, 0.25f);
    }
}
