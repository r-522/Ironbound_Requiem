// 役割: ヒット時に Time.timeScale を瞬間的に落として手応えを演出。
using System.Collections;
using UnityEngine;

namespace Ironbound.Combat
{
    public class HitStopService : MonoBehaviour
    {
        public static HitStopService Instance { get; private set; }
        private Coroutine _co;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
        }

        public static void Ensure()
        {
            if (Instance != null) return;
            new GameObject("HitStopService").AddComponent<HitStopService>();
        }

        public void Freeze(float milliseconds, float scale = 0.05f)
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Routine(milliseconds / 1000f, scale));
        }

        private IEnumerator Routine(float sec, float scale)
        {
            Time.timeScale = scale;
            float t = 0;
            while (t < sec) { t += Time.unscaledDeltaTime; yield return null; }
            Time.timeScale = 1f;
            _co = null;
        }
    }
}
