// 役割: 設置済みタワーの本体。攻撃/支援/壁/特殊を Category で分岐。
using UnityEngine;
using Ironbound.AI;
using Ironbound.Combat;
using Ironbound.Core;
using Ironbound.Data;
using Ironbound.Audio;

namespace Ironbound.Towers
{
    public class TowerComponent : MonoBehaviour, ITargetable
    {
        public TowerData Data;
        [SerializeField] private HealthComponent health;
        private float _cooldown;
        public Transform Transform => transform;
        public TargetCategory Category =>
            Data?.Category == TowerCategory.Wall ? TargetCategory.Barricade :
            Data?.Category == TowerCategory.Support ? TargetCategory.SupportTower : TargetCategory.Tower;
        public bool IsAlive => health != null && !health.IsDead;

        public void Initialize(TowerData data)
        {
            Data = data;
            if (health == null) health = GetComponent<HealthComponent>();
            if (health == null) health = gameObject.AddComponent<HealthComponent>();
            health.Configure(data.Durability);
            health.OnDied += OnDied;
            TargetRegistry.Register(this);
            EventBus.Publish(new TowerBuiltEvent { Tower = gameObject, TowerId = data.TowerId });
        }

        private void OnDied(GameObject killer)
        {
            EventBus.Publish(new TowerDestroyedEvent { Tower = gameObject });
            TargetRegistry.Unregister(this);
            Destroy(gameObject, 0.05f);
        }

        private void OnDestroy() { TargetRegistry.Unregister(this); }

        public void Repair(float amount) => health?.Heal(amount);

        private void Update()
        {
            if (Data == null) return;
            _cooldown -= Time.deltaTime;
            switch (Data.Category)
            {
                case TowerCategory.Attack:   TickAttack(); break;
                case TowerCategory.Support:  TickSupport(); break;
                case TowerCategory.Special:  TickSpecial(); break;
                case TowerCategory.Wall:     /* 壁は被弾耐久のみ */ break;
            }
        }

        private void TickAttack()
        {
            if (_cooldown > 0) return;
            var enemy = TowerTargeting.FindEnemyInRange(transform.position, Data.Range);
            if (enemy == null) return;
            _cooldown = 1f / Mathf.Max(0.1f, Data.AttackRate);
            transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
            TowerProjectile.Fire(transform.position + Vector3.up * 1.4f, enemy, Data, gameObject);
            if (Data.SfxCue != null) AudioManager.Instance?.PlayCue(Data.SfxCue, transform.position);
        }

        private void TickSupport()
        {
            if (_cooldown > 0) return;
            _cooldown = 1f;
            // 周辺の味方/壁/塔を回復
            var cols = Physics.OverlapSphere(transform.position, Data.Range);
            foreach (var c in cols)
            {
                var hp = c.GetComponentInParent<HealthComponent>();
                if (hp == null || hp.IsDead) continue;
                var ene = c.GetComponentInParent<EnemyAIController>();
                if (ene != null) continue;
                hp.Heal(Data.Damage); // Damage を回復量として使用
            }
        }

        private void TickSpecial()
        {
            if (_cooldown > 0) return;
            _cooldown = 1f / Mathf.Max(0.1f, Data.AttackRate);
            var cols = Physics.OverlapSphere(transform.position, Data.Range);
            foreach (var c in cols)
            {
                var ene = c.GetComponentInParent<EnemyAIController>();
                if (ene == null) continue;
                var hp = ene.GetComponent<HealthComponent>();
                if (hp == null) continue;
                hp.ApplyDamage(Data.Damage * 0.6f, gameObject);
                // Frost/Gravity 風のスロー
                if (ene.Agent != null) ene.Agent.speed = Mathf.Max(0.5f, ene.Data.MoveSpeed * 0.6f);
            }
        }
    }
}
