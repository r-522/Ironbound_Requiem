// 役割: 敵タイプ別行動の戦略インターフェース。
using UnityEngine;

namespace Ironbound.AI
{
    public interface IEnemyBehavior
    {
        void Tick(EnemyAIController ctrl, float dt);
    }
}
