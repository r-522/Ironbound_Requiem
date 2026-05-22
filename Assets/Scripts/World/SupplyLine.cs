// 役割: 補給線拠点。被弾するとミッション失敗判定。
using UnityEngine;
using Ironbound.AI;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.World
{
    [RequireComponent(typeof(HealthComponent))]
    public class SupplyLine : MonoBehaviour, ITargetable
    {
        private HealthComponent _hp;
        public Transform Transform => transform;
        public TargetCategory Category => TargetCategory.SupplyLine;
        public bool IsAlive => _hp != null && !_hp.IsDead;

        private void Awake() { _hp = GetComponent<HealthComponent>(); _hp.Configure(300f); }
        private void OnEnable() => TargetRegistry.Register(this);
        private void OnDisable() => TargetRegistry.Unregister(this);
    }
}
