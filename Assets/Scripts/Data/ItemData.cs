// 役割: アイテム定義(MVPでは簡易インベントリ用)。
using UnityEngine;

namespace Ironbound.Data
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum ItemType { Weapon, Armor, Consumable, Material, Quest }

    [System.Serializable]
    public struct Affix { public string Stat; public float Value; }

    [CreateAssetMenu(menuName = "Ironbound/Item")]
    public class ItemData : ScriptableObject
    {
        public string ItemId;
        public string Name;
        public ItemRarity Rarity;
        public ItemType Type;
        public Affix[] Affixes;
        public DamageElement Element;
        public Sprite Icon;
        public GameObject ModelPrefab;
    }
}
