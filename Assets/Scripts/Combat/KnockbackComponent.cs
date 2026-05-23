// 役割: ノックバック適用(CharacterController / Rigidbody / NavMeshAgent をサポート)。
using UnityEngine;
using UnityEngine.AI;

namespace Ironbound.Combat
{
    public class KnockbackComponent : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _decay = 6f;
        private CharacterController _cc;
        private Rigidbody _rb;
        private NavMeshAgent _agent;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _rb = GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Apply(Vector3 direction, float force)
        {
            _velocity = direction.normalized * force;
        }

        private void Update()
        {
            if (_velocity.sqrMagnitude < 0.001f) return;
            Vector3 step = _velocity * Time.deltaTime;
            if (_cc != null && _cc.enabled) _cc.Move(step);
            else if (_rb != null) _rb.AddForce(_velocity, ForceMode.VelocityChange);
            else if (_agent != null && _agent.isOnNavMesh) _agent.Move(step);
            else transform.position += step;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, _decay * Time.deltaTime);
        }
    }
}
