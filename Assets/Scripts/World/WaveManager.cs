// 役割: WaveData に従い敵を生成し、撃破完了でウェーブ完了を通知。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironbound.AI;
using Ironbound.Core;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.World
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private SpawnPoint[] spawnPoints;
        [SerializeField] private Transform player;
        public int Active { get; private set; }
        public bool IsRunning { get; private set; }
        public int CurrentWaveIndex { get; private set; }

        public IEnumerator Run(WaveData wave)
        {
            if (wave == null) yield break;
            IsRunning = true;
            CurrentWaveIndex++;
            EventBus.Publish(new WaveStartedEvent { WaveIndex = CurrentWaveIndex });
            yield return new WaitForSeconds(wave.StartDelay);
            var co = StartCoroutine(SpawnAll(wave));
            yield return co;
            while (Active > 0) yield return null;
            IsRunning = false;
            EventBus.Publish(new WaveClearedEvent { WaveIndex = CurrentWaveIndex });
        }

        private IEnumerator SpawnAll(WaveData wave)
        {
            foreach (var s in wave.Spawns)
            {
                for (int i = 0; i < s.Count; i++)
                {
                    SpawnOne(s.Enemy, s.SpawnPointIndex);
                    yield return new WaitForSeconds(s.Interval);
                }
            }
        }

        private void SpawnOne(EnemyData data, int sp)
        {
            if (data == null) return;
            var point = (spawnPoints != null && spawnPoints.Length > 0)
                ? spawnPoints[Mathf.Clamp(sp, 0, spawnPoints.Length - 1)].transform
                : transform;
            GameObject go = data.Prefab != null ? Instantiate(data.Prefab) : BuildPrimitiveEnemy(data);
            go.transform.position = point.position;
            var ai = go.GetComponent<EnemyAIController>() ?? go.AddComponent<EnemyAIController>();
            EnsureAux(go);
            ai.Initialize(data, player);
            Active++;
            ai.GetComponent<HealthComponent>().OnDied += _ => Active = Mathf.Max(0, Active - 1);
        }

        private static GameObject BuildPrimitiveEnemy(EnemyData d)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy_" + d.EnemyId;
            var rend = go.GetComponent<Renderer>();
            if (rend != null && rend.material.HasProperty("_BaseColor")) rend.material.SetColor("_BaseColor", d.BodyTint);
            return go;
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
