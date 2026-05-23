// 役割: ミッション=フェーズ列。各フェーズに目標とウェーブを束ねる。
using UnityEngine;

namespace Ironbound.Data
{
    public enum ObjectiveType { Explore, BuildTower, SurviveWave, CaptureZone, KillBoss }

    [System.Serializable]
    public struct PhaseDefinition
    {
        public string PhaseName;
        public ObjectiveType Objective;
        public WaveData[] Waves;
        public float TimeBudget;     // 0 = 制限なし
        public string ObjectiveLabel;
    }

    [CreateAssetMenu(menuName = "Ironbound/Mission")]
    public class MissionData : ScriptableObject
    {
        public string MissionId;
        public string Name;
        public string MapId;
        public PhaseDefinition[] Phases;
        public EnemyData BossEnemy;
        public LootTableData RewardTable;
        public float TimeLimit = 900f;
    }
}
