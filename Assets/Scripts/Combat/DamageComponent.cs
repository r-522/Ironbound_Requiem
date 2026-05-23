// 役割: ダメージ計算のユーティリティ。元素相性と防御を反映。
using UnityEngine;
using Ironbound.Data;

namespace Ironbound.Combat
{
    public static class DamageComponent
    {
        // 基本式: (base * elementMul) - armor*0.5, 最低 1。Armor が大きいと割合的に軽減。
        public static float Compute(float baseDamage, DamageElement element, float armor, DamageElement enemyWeakness)
        {
            float mul = 1f;
            if (element != DamageElement.Physical && element == enemyWeakness) mul = 1.5f;
            float dmg = baseDamage * mul - armor * 0.5f;
            return Mathf.Max(1f, dmg);
        }
    }
}
