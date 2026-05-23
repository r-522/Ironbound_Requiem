// 役割: 最寄り敵をロックオン。カメラ向きを補助。
using UnityEngine;
using Ironbound.Combat;

namespace Ironbound.Player
{
    public class LockOnController : MonoBehaviour
    {
        [SerializeField] private float radius = 14f;
        [SerializeField] private LayerMask enemyMask;
        public Transform Current { get; private set; }

        public void Toggle()
        {
            if (Current != null) { Current = null; return; }
            float best = float.MaxValue;
            Transform pick = null;
            var hits = Physics.OverlapSphere(transform.position, radius, enemyMask, QueryTriggerInteraction.Ignore);
            foreach (var h in hits)
            {
                var hp = h.GetComponentInParent<HealthComponent>();
                if (hp == null || hp.IsDead) continue;
                float d = (h.transform.position - transform.position).sqrMagnitude;
                if (d < best) { best = d; pick = hp.transform; }
            }
            Current = pick;
        }

        private void Update()
        {
            if (Current == null) return;
            var hp = Current.GetComponent<HealthComponent>();
            if (hp == null || hp.IsDead) Current = null;
        }
    }
}
