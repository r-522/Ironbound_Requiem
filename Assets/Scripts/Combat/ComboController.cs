// 役割: 通常攻撃のチェイン管理。連打タイミング窓と硬直を管理する。
using UnityEngine;
using Ironbound.Data;

namespace Ironbound.Combat
{
    public class ComboController : MonoBehaviour
    {
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private float chainWindow = 0.55f;
        [SerializeField] private float recovery = 0.32f;
        public int Step { get; private set; }
        public bool IsAttacking { get; private set; }
        public float ChainTimer { get; private set; }
        private float _cooldown;
        private SkillData[] _chain;

        public void Configure(SkillData basicAttack, LayerMask mask)
        {
            // MVPでは同じ basic を 3 連参照(後で差し替え可能)
            _chain = new[] { basicAttack, basicAttack, basicAttack };
            hitMask = mask;
        }

        public bool TryAttack(Transform origin, GameObject owner)
        {
            if (_cooldown > 0) return false;
            if (_chain == null || _chain.Length == 0 || _chain[0] == null) return false;
            int next = (ChainTimer > 0) ? (Step % _chain.Length) : 0;
            Step = next + 1;
            ChainTimer = chainWindow;
            _cooldown = recovery;
            IsAttacking = true;
            MeleeHitbox.Strike(origin, _chain[next], owner, hitMask);
            return true;
        }

        private void Update()
        {
            if (_cooldown > 0) { _cooldown -= Time.deltaTime; if (_cooldown <= 0) IsAttacking = false; }
            if (ChainTimer > 0) { ChainTimer -= Time.deltaTime; if (ChainTimer <= 0) Step = 0; }
        }
    }
}
