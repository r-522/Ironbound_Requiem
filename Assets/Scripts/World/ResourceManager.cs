// 役割: 資源 (建築素材) の保有・取得・消費を管理。
using UnityEngine;
using Ironbound.Core;

namespace Ironbound.World
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }
        [SerializeField] private int current = 80;
        public int Current => current;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
            EventBus.Publish(new ResourceChangedEvent { Current = current, Delta = 0 });
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void Add(int amount)
        {
            current += amount;
            EventBus.Publish(new ResourceChangedEvent { Current = current, Delta = amount });
        }

        public bool TrySpend(int amount)
        {
            if (current < amount) return false;
            current -= amount;
            EventBus.Publish(new ResourceChangedEvent { Current = current, Delta = -amount });
            return true;
        }
    }
}
