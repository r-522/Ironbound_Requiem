// 役割: タワー側の敵検索ユーティリティ。
using UnityEngine;
using Ironbound.AI;

namespace Ironbound.Towers
{
    public static class TowerTargeting
    {
        public static Transform FindEnemyInRange(Vector3 origin, float range)
        {
            float sqr = range * range;
            Transform best = null; float bestD = float.MaxValue;
            // EnemyAIController を Scene 全体から拾うのは重いので軽量検索
            var found = Object.FindObjectsByType<EnemyAIController>(FindObjectsSortMode.None);
            foreach (var e in found)
            {
                if (e == null) continue;
                float d = (e.transform.position - origin).sqrMagnitude;
                if (d > sqr || d >= bestD) continue;
                bestD = d; best = e.transform;
            }
            return best;
        }
    }
}
