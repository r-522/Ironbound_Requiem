// 役割: MissionData のフェーズを順に駆動。中ボス撃破で勝利。
using System.Collections;
using UnityEngine;
using Ironbound.Core;
using Ironbound.Combat;
using Ironbound.Data;
using Ironbound.AI;

namespace Ironbound.World
{
    public class MissionManager : MonoBehaviour
    {
        public MissionData Mission;
        [SerializeField] private WaveManager wave;
        [SerializeField] private Transform bossSpawn;
        [SerializeField] private Transform player;
        public int PhaseIndex { get; private set; }
        public float Elapsed { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsVictory { get; private set; }

        private void Start()
        {
            if (Mission == null) return;
            StartCoroutine(RunMission());
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnDestroy() => EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);

        private void Update() { if (IsRunning) Elapsed += Time.deltaTime; }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            IsRunning = false; IsVictory = false;
            EventBus.Publish(new MissionResultEvent { Victory = false, Duration = Elapsed, Score = 0 });
            GameStateMachine.GoResult();
        }

        private IEnumerator RunMission()
        {
            IsRunning = true;
            for (int i = 0; i < Mission.Phases.Length; i++)
            {
                PhaseIndex = i;
                var ph = Mission.Phases[i];
                EventBus.Publish(new PhaseChangedEvent { PhaseIndex = i, PhaseName = ph.PhaseName });
                yield return StartCoroutine(RunPhase(ph));
                if (!IsRunning) yield break;
            }
            IsVictory = true; IsRunning = false;
            EventBus.Publish(new MissionResultEvent { Victory = true, Duration = Elapsed, Score = 1000 });
            GameStateMachine.GoResult();
        }

        private IEnumerator RunPhase(PhaseDefinition ph)
        {
            switch (ph.Objective)
            {
                case ObjectiveType.Explore:
                case ObjectiveType.BuildTower:
                    float t = ph.TimeBudget > 0 ? ph.TimeBudget : 25f;
                    while (t > 0 && IsRunning) { t -= Time.deltaTime; yield return null; }
                    break;
                case ObjectiveType.SurviveWave:
                    if (ph.Waves != null)
                    {
                        foreach (var w in ph.Waves)
                        {
                            yield return StartCoroutine(wave.Run(w));
                            if (!IsRunning) yield break;
                        }
                    }
                    break;
                case ObjectiveType.CaptureZone:
                    yield return new WaitForSeconds(8f);
                    break;
                case ObjectiveType.KillBoss:
                    yield return StartCoroutine(SpawnAndAwaitBoss());
                    break;
            }
        }

        private IEnumerator SpawnAndAwaitBoss()
        {
            if (Mission.BossEnemy == null) yield break;
            var data = Mission.BossEnemy;
            var go = data.Prefab != null ? Instantiate(data.Prefab) : GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Boss_" + data.EnemyId;
            go.transform.localScale = Vector3.one * 2.4f;
            go.transform.position = bossSpawn != null ? bossSpawn.position : transform.position + Vector3.forward * 12f;
            EnsureAux(go);
            var ai = go.GetComponent<EnemyAIController>() ?? go.AddComponent<EnemyAIController>();
            ai.Initialize(data, player);
            var hp = go.GetComponent<HealthComponent>();
            bool dead = false;
            hp.OnDied += _ => dead = true;
            while (!dead && IsRunning) yield return null;
        }

        private static void EnsureAux(GameObject go)
        {
            if (go.GetComponent<HealthComponent>() == null) go.AddComponent<HealthComponent>();
            if (go.GetComponent<AggroComponent>() == null) go.AddComponent<AggroComponent>();
            if (go.GetComponent<HitReactionComponent>() == null) go.AddComponent<HitReactionComponent>();
            if (go.GetComponent<KnockbackComponent>() == null) go.AddComponent<KnockbackComponent>();
            if (go.GetComponent<UnityEngine.AI.NavMeshAgent>() == null) go.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }
    }
}
