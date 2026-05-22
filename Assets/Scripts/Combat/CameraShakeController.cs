// 役割: メインカメラに揺れを与える(Cinemachine 不在時の汎用フォールバック)。
using UnityEngine;

namespace Ironbound.Combat
{
    public class CameraShakeController : MonoBehaviour
    {
        public static CameraShakeController Instance { get; private set; }
        private float _amp, _freq, _time;
        private Vector3 _baseLocal;

        private void Awake()
        {
            Instance = this;
            _baseLocal = transform.localPosition;
        }

        public static void Shake(float amp = 0.2f, float duration = 0.15f, float frequency = 40f)
        {
            if (Instance == null) return;
            Instance._amp = amp;
            Instance._time = duration;
            Instance._freq = frequency;
        }

        private void LateUpdate()
        {
            if (_time > 0)
            {
                _time -= Time.unscaledDeltaTime;
                float t = Time.unscaledTime * _freq;
                Vector3 off = new Vector3(Mathf.PerlinNoise(t, 0) - 0.5f,
                                          Mathf.PerlinNoise(0, t) - 0.5f,
                                          0) * _amp * 2f;
                transform.localPosition = _baseLocal + off;
            }
            else if (transform.localPosition != _baseLocal)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _baseLocal, 12f * Time.unscaledDeltaTime);
            }
        }
    }
}
