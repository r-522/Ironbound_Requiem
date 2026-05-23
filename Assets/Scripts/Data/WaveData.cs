// 役割: 1 ウェーブの構成(出現敵/数/間隔)。
using UnityEngine;

namespace Ironbound.Data
{
    [System.Serializable]
    public struct WaveSpawn
    {
        public EnemyData Enemy;
        public int Count;
        public float Interval;
        public int SpawnPointIndex;
    }

    [CreateAssetMenu(menuName = "Ironbound/Wave")]
    public class WaveData : ScriptableObject
    {
        public string WaveId;
        public string Label;
        public float StartDelay = 3f;
        public WaveSpawn[] Spawns;
    }
}
