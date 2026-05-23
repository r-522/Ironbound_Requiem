// 役割: マウスでヨー/ピッチ、ターゲット背後に追従するサードパーソンカメラ。
using UnityEngine;

namespace Ironbound.Player
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        public Transform Target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private float distance = 5.0f;
        [SerializeField] private float sensitivity = 2.4f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private LayerMask collideMask = ~0;
        public float Yaw { get; private set; }
        public float Pitch { get; private set; } = 18f;

        public void AddLook(Vector2 delta)
        {
            Yaw += delta.x * sensitivity;
            Pitch = Mathf.Clamp(Pitch - delta.y * sensitivity, minPitch, maxPitch);
        }

        public Vector3 ForwardFlat
        {
            get { var f = Quaternion.Euler(0, Yaw, 0) * Vector3.forward; f.y = 0; return f.normalized; }
        }

        private void LateUpdate()
        {
            if (Target == null) return;
            Quaternion rot = Quaternion.Euler(Pitch, Yaw, 0);
            Vector3 pivot = Target.position + offset;
            Vector3 desired = pivot - rot * Vector3.forward * distance;
            if (Physics.SphereCast(pivot, collisionRadius, (desired - pivot).normalized, out var hit,
                                   distance, collideMask, QueryTriggerInteraction.Ignore))
            {
                desired = pivot + (desired - pivot).normalized * Mathf.Max(1.2f, hit.distance - 0.1f);
            }
            transform.position = desired;
            transform.rotation = rot;
        }
    }
}
