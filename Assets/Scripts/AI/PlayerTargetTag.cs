// 役割: Player を ITargetable として TargetRegistry へ自動登録。
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.AI
{
    public class PlayerTargetTag : MonoBehaviour, ITargetable
    {
        public Transform Transform => transform;
        public TargetCategory Category => TargetCategory.Player;
        private HealthComponent _hp;
        public bool IsAlive => _hp != null && !_hp.IsDead;

        private void OnEnable() { _hp = GetComponent<HealthComponent>(); TargetRegistry.Register(this); }
        private void OnDisable() { TargetRegistry.Unregister(this); }
    }
}
