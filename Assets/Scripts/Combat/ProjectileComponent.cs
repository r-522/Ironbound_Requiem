// 役割: 直進する弾。寿命と命中処理を担う。タワー・Ranger・Arcane で共用。
using UnityEngine;
using Ironbound.Data;
using Ironbound.Core;

namespace Ironbound.Combat
{
    public class ProjectileComponent : MonoBehaviour
    {
        public float Speed = 22f;
        public float Damage = 12f;
        public float Lifetime = 3f;
        public float Knockback = 1.2f;
        public DamageElement Element;
        public GameObject Owner;
        public LayerMask HitMask = ~0;
        private float _age;

        public void Launch(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age > Lifetime) { Destroy(gameObject); return; }
            float step = Speed * Time.deltaTime;
            if (Physics.SphereCast(transform.position, 0.18f, transform.forward, out var hit, step, HitMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject != Owner)
                {
                    var hp = hit.collider.GetComponentInParent<HealthComponent>();
                    if (hp != null)
                    {
                        float dmg = DamageComponent.Compute(Damage, Element, 0, DamageElement.Physical);
                        hp.ApplyDamage(dmg, Owner);
                        var kb = hp.GetComponent<KnockbackComponent>();
                        if (kb != null) kb.Apply(transform.forward, Knockback);
                        EventBus.Publish(new DamageDealtEvent { Source = Owner, Target = hp.gameObject, Amount = dmg });
                    }
                    Destroy(gameObject);
                    return;
                }
            }
            transform.position += transform.forward * step;
        }
    }
}
