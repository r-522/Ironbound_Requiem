// 役割: 敵タイプ定義 SO。AI 種別、ターゲット優先度、ステータス。
using UnityEngine;

namespace Ironbound.Data
{
    public enum EnemyType { Swarm, Tank, Siege, Flying, Assassin, Shaman, Archer, Bomber, Commander, Boss }
    public enum TargetCategory { Player, Tower, SupportTower, Barricade, SupplyLine, Stronghold }

    [CreateAssetMenu(menuName = "Ironbound/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public string EnemyId;
        public string Name;
        public EnemyType Type;
        public float HP = 60f;
        public float Damage = 10f;
        public float Armor = 0f;
        public float MoveSpeed = 3.2f;
        public float AttackRange = 1.8f;
        public float AttackRate = 1f;
        public TargetCategory[] TargetPriority;
        public GameObject Prefab;
        public AudioCueData FootstepCue;
        public AudioCueData DeathCue;
        public Color BodyTint = new Color(0.2f, 0.2f, 0.22f);
        public float CommanderBuff = 0f;
    }
}
