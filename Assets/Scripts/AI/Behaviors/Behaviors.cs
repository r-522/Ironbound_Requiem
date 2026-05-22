// 役割: 10 敵タイプの行動戦略を 1 ファイルに集約(各 50 行未満で簡潔に)。
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.AI.Behaviors
{
    public static class BehaviorFactory
    {
        public static IEnemyBehavior Create(EnemyType t) => t switch
        {
            EnemyType.Swarm => new SwarmBehavior(),
            EnemyType.Tank => new TankBehavior(),
            EnemyType.Siege => new SiegeBehavior(),
            EnemyType.Flying => new FlyingBehavior(),
            EnemyType.Assassin => new AssassinBehavior(),
            EnemyType.Shaman => new ShamanBehavior(),
            EnemyType.Archer => new ArcherBehavior(),
            EnemyType.Bomber => new BomberBehavior(),
            EnemyType.Commander => new CommanderBehavior(),
            EnemyType.Boss => new BossBehavior(),
            _ => new SwarmBehavior()
        };
    }

    // 共通ヘルパ
    internal static class B
    {
        public static void ChaseAndStrike(EnemyAIController c)
        {
            var target = ResolveTarget(c);
            if (target == null) return;
            if (c.Agent.isOnNavMesh) c.Agent.SetDestination(target.position);
            float d = Vector3.Distance(c.transform.position, target.position);
            if (d <= c.Data.AttackRange)
            {
                var t = target.GetComponent<HealthComponent>();
                if (t != null) c.TryAttack(new SimpleTarget(target, TargetCategory.Player, !t.IsDead));
            }
        }

        public static Transform ResolveTarget(EnemyAIController c)
        {
            var ag = c.GetComponent<AggroComponent>();
            if (ag != null && ag.Current != null) return ag.Current;
            return c.Player;
        }

        public class SimpleTarget : ITargetable
        {
            public Transform Transform { get; }
            public TargetCategory Category { get; }
            public bool IsAlive { get; }
            public SimpleTarget(Transform t, TargetCategory cat, bool alive) { Transform = t; Category = cat; IsAlive = alive; }
        }
    }

    public class SwarmBehavior : IEnemyBehavior { public void Tick(EnemyAIController c, float dt) => B.ChaseAndStrike(c); }
    public class TankBehavior : IEnemyBehavior { public void Tick(EnemyAIController c, float dt) => B.ChaseAndStrike(c); }
    public class SiegeBehavior : IEnemyBehavior { public void Tick(EnemyAIController c, float dt) => B.ChaseAndStrike(c); }

    public class FlyingBehavior : IEnemyBehavior
    {
        public void Tick(EnemyAIController c, float dt)
        {
            B.ChaseAndStrike(c);
            // 軽量な上下浮遊
            var p = c.transform.position;
            p.y = Mathf.Lerp(p.y, 2.5f, dt * 2f);
            c.transform.position = p;
        }
    }

    public class AssassinBehavior : IEnemyBehavior
    {
        private float _dashTimer;
        public void Tick(EnemyAIController c, float dt)
        {
            _dashTimer -= dt;
            if (_dashTimer <= 0 && c.Player != null && Vector3.Distance(c.transform.position, c.Player.position) > 4f)
            {
                c.Agent.speed = c.Data.MoveSpeed * 1.8f;
                _dashTimer = 3f;
            }
            else c.Agent.speed = c.Data.MoveSpeed;
            B.ChaseAndStrike(c);
        }
    }

    public class ShamanBehavior : IEnemyBehavior
    {
        private float _healTimer;
        public void Tick(EnemyAIController c, float dt)
        {
            _healTimer -= dt;
            B.ChaseAndStrike(c);
            // 後方維持: ターゲットから 6m を保つ
            var t = B.ResolveTarget(c);
            if (t != null)
            {
                Vector3 away = (c.transform.position - t.position).normalized * 6f + t.position;
                if (Vector3.Distance(c.transform.position, t.position) < 5f && c.Agent.isOnNavMesh)
                    c.Agent.SetDestination(away);
            }
        }
    }

    public class ArcherBehavior : IEnemyBehavior
    {
        public void Tick(EnemyAIController c, float dt)
        {
            var t = B.ResolveTarget(c);
            if (t == null) return;
            float d = Vector3.Distance(c.transform.position, t.position);
            if (d > c.Data.AttackRange * 0.9f && c.Agent.isOnNavMesh) c.Agent.SetDestination(t.position);
            else
            {
                c.Agent.ResetPath();
                c.transform.LookAt(new Vector3(t.position.x, c.transform.position.y, t.position.z));
                if (c.AttackCooldown <= 0)
                {
                    var hp = t.GetComponent<HealthComponent>();
                    if (hp != null) c.TryAttack(new B.SimpleTarget(t, TargetCategory.Player, !hp.IsDead));
                }
            }
        }
    }

    public class BomberBehavior : IEnemyBehavior
    {
        public void Tick(EnemyAIController c, float dt)
        {
            B.ChaseAndStrike(c);
            var t = B.ResolveTarget(c);
            if (t != null && Vector3.Distance(c.transform.position, t.position) < 1.6f)
            {
                // 自爆: 周囲 3m に範囲ダメ
                var cols = Physics.OverlapSphere(c.transform.position, 3f);
                foreach (var col in cols)
                {
                    var hp = col.GetComponentInParent<HealthComponent>();
                    if (hp != null && hp.gameObject != c.gameObject) hp.ApplyDamage(c.Data.Damage * 1.5f, c.gameObject);
                }
                Object.Destroy(c.gameObject);
            }
        }
    }

    public class CommanderBehavior : IEnemyBehavior
    {
        private float _auraTimer;
        public void Tick(EnemyAIController c, float dt)
        {
            _auraTimer -= dt;
            B.ChaseAndStrike(c);
            if (_auraTimer <= 0)
            {
                _auraTimer = 1.5f;
                var cols = Physics.OverlapSphere(c.transform.position, 8f);
                foreach (var col in cols)
                {
                    var ally = col.GetComponentInParent<EnemyAIController>();
                    if (ally != null && ally != c) ally.DamageBonus = 1.25f;
                }
            }
        }
    }

    public class BossBehavior : IEnemyBehavior
    {
        private float _phaseTimer;
        private int _phase;
        public void Tick(EnemyAIController c, float dt)
        {
            _phaseTimer -= dt;
            B.ChaseAndStrike(c);
            if (_phaseTimer <= 0)
            {
                _phaseTimer = 6f;
                _phase = (_phase + 1) % 3;
                // フェーズ 1: 突進、2: 範囲、3: 通常
                if (_phase == 0 && c.Player != null)
                {
                    Vector3 dir = (c.Player.position - c.transform.position).normalized;
                    var kb = c.GetComponent<KnockbackComponent>(); kb?.Apply(dir, 12f);
                }
                else if (_phase == 1)
                {
                    var cols = Physics.OverlapSphere(c.transform.position, 5f);
                    foreach (var col in cols)
                    {
                        var hp = col.GetComponentInParent<HealthComponent>();
                        if (hp != null && hp.gameObject != c.gameObject) hp.ApplyDamage(c.Data.Damage * 0.6f, c.gameObject);
                    }
                    CameraShakeController.Shake(0.35f, 0.4f);
                }
            }
        }
    }
}
