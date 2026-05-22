// 役割: NavMeshAgent と AggroComponent を束ねて敵を駆動。タイプごとの行動は IEnemyBehavior に委譲。
using UnityEngine;
using UnityEngine.AI;
using Ironbound.Combat;
using Ironbound.Core;
using Ironbound.Data;
using Ironbound.AI.Behaviors;
using Ironbound.Audio;

namespace Ironbound.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAIController : MonoBehaviour
    {
        public EnemyData Data;
        [SerializeField] private HealthComponent health;
        [SerializeField] private AggroComponent aggro;
        [SerializeField] private HitReactionComponent react;
        public NavMeshAgent Agent { get; private set; }
        public float AttackCooldown;
        public float DamageBonus = 1f;        // Commander オーラ
        public float ArmorBonus = 0f;
        public IEnemyBehavior Behavior;
        private float _retargetTimer;
        private bool _isFar;
        public Transform Player;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            if (health == null) health = GetComponent<HealthComponent>();
            if (aggro == null) aggro = GetComponent<AggroComponent>();
            if (react == null) react = GetComponent<HitReactionComponent>();
        }

        public void Initialize(EnemyData data, Transform player)
        {
            Data = data;
            Player = player;
            if (health != null) health.Configure(data.HP);
            if (Agent != null)
            {
                Agent.speed = data.MoveSpeed;
                Agent.stoppingDistance = Mathf.Max(0.5f, data.AttackRange - 0.3f);
            }
            Behavior = BehaviorFactory.Create(data.Type);
            if (health != null) health.OnDied += HandleDeath;
        }

        private void HandleDeath(GameObject killer)
        {
            EventBus.Publish(new EnemyKilledEvent { Enemy = gameObject, LootSeed = Random.Range(0, int.MaxValue) });
            if (Data?.DeathCue != null) AudioManager.Instance?.PlayCue(Data.DeathCue, transform.position);
            Destroy(gameObject, 0.05f);
        }

        private void Update()
        {
            if (Data == null || health == null || health.IsDead) return;
            float dt = Time.deltaTime;
            AttackCooldown -= dt;
            _retargetTimer -= dt;

            // 遠距離は更新間引き
            if (Player != null) _isFar = (transform.position - Player.position).sqrMagnitude > 60f * 60f;
            if (_isFar && Time.frameCount % 5 != 0) return;

            if (_retargetTimer <= 0f)
            {
                aggro?.Refresh(transform.position, Data.TargetPriority);
                _retargetTimer = 0.6f;
            }
            if (react != null && react.StaggerTime > 0) { Agent.isStopped = true; return; }
            Agent.isStopped = false;

            Behavior?.Tick(this, dt);
        }

        public void TryAttack(ITargetable target)
        {
            if (AttackCooldown > 0 || target == null || !target.IsAlive) return;
            var hp = target.Transform.GetComponent<HealthComponent>();
            if (hp == null) return;
            float dmg = DamageComponent.Compute(Data.Damage * DamageBonus, DamageElement.Physical, 0, DamageElement.Physical);
            hp.ApplyDamage(dmg, gameObject);
            AttackCooldown = 1f / Mathf.Max(0.1f, Data.AttackRate);
        }
    }
}
