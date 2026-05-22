// 役割: タワー定義 SO。建築コスト、攻撃仕様、耐久、外見プレハブ等。
using UnityEngine;

namespace Ironbound.Data
{
    public enum TowerCategory { Wall, Attack, Support, Special }

    [CreateAssetMenu(menuName = "Ironbound/Tower")]
    public class TowerData : ScriptableObject
    {
        public string TowerId;
        public string Name;
        public TowerCategory Category;
        public int Cost = 25;
        public float Range = 8f;
        public float Damage = 10f;
        public float AttackRate = 1.2f;   // shots/sec
        public float Durability = 200f;
        public DamageElement Element = DamageElement.Physical;
        public float BuildTime = 0.5f;
        public string[] UpgradePath;
        public GameObject Prefab;
        public GameObject ProjectilePrefab;
        public AudioCueData SfxCue;
        public Color UiTint = new Color(0.7f, 0.6f, 0.3f);
        [TextArea] public string Description;
    }
}
