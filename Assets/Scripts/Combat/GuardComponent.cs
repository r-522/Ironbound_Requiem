// 役割: ガード(RMB長押し)。ガード中は被ダメ軽減 + ノックバック軽減。
using UnityEngine;

namespace Ironbound.Combat
{
    public class GuardComponent : MonoBehaviour
    {
        public bool IsGuarding { get; private set; }
        [SerializeField] private float damageReduction = 0.6f;
        [SerializeField] private float staminaPerSec = 6f;
        [SerializeField] private StaminaComponent stamina;

        private void Reset() { stamina = GetComponent<StaminaComponent>(); }

        public void SetGuard(bool on) { IsGuarding = on; }

        private void Update()
        {
            if (IsGuarding && stamina != null)
            {
                if (!stamina.TrySpend(staminaPerSec * Time.deltaTime)) IsGuarding = false;
            }
        }

        public float ModifyIncoming(float raw) => IsGuarding ? raw * (1f - damageReduction) : raw;
    }
}
