// 役割: 型安全な発火/購読型 EventBus。直接参照を増やさずに通知を伝搬。
using System;
using System.Collections.Generic;

namespace Ironbound.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            _handlers.TryGetValue(typeof(T), out var d);
            _handlers[typeof(T)] = Delegate.Combine(d, handler);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (!_handlers.TryGetValue(typeof(T), out var d)) return;
            var nd = Delegate.Remove(d, handler);
            if (nd == null) _handlers.Remove(typeof(T));
            else _handlers[typeof(T)] = nd;
        }

        public static void Publish<T>(T evt)
        {
            if (_handlers.TryGetValue(typeof(T), out var d))
                ((Action<T>)d)?.Invoke(evt);
        }

        public static void Clear() => _handlers.Clear();
    }

    // 共通イベント定義
    public struct DamageDealtEvent { public UnityEngine.GameObject Source, Target; public float Amount; }
    public struct EnemyKilledEvent { public UnityEngine.GameObject Enemy; public int LootSeed; }
    public struct PlayerDiedEvent { public UnityEngine.GameObject Player; }
    public struct ResourceChangedEvent { public int Current, Delta; }
    public struct WaveStartedEvent { public int WaveIndex; }
    public struct WaveClearedEvent { public int WaveIndex; }
    public struct PhaseChangedEvent { public int PhaseIndex; public string PhaseName; }
    public struct MissionResultEvent { public bool Victory; public float Duration; public int Score; }
    public struct TowerBuiltEvent { public UnityEngine.GameObject Tower; public string TowerId; }
    public struct TowerDestroyedEvent { public UnityEngine.GameObject Tower; }
}
