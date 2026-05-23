// 役割: 抽選テーブル。重み付きランダム。
using UnityEngine;

namespace Ironbound.Data
{
    [System.Serializable]
    public struct LootEntry { public ItemData Item; public int Weight; public int MinResource, MaxResource; }

    [CreateAssetMenu(menuName = "Ironbound/LootTable")]
    public class LootTableData : ScriptableObject
    {
        public LootEntry[] Entries;

        public LootEntry Roll(System.Random rng)
        {
            int total = 0;
            foreach (var e in Entries) total += Mathf.Max(0, e.Weight);
            if (total <= 0 || Entries.Length == 0) return default;
            int v = rng.Next(0, total);
            int acc = 0;
            foreach (var e in Entries)
            {
                acc += Mathf.Max(0, e.Weight);
                if (v < acc) return e;
            }
            return Entries[Entries.Length - 1];
        }
    }
}
