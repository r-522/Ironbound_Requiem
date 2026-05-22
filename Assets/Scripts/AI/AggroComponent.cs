// 役割: ターゲット候補をシーンから列挙し、優先度に従い 1 体を選ぶ。
using System.Collections.Generic;
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.AI
{
    public interface ITargetable
    {
        Transform Transform { get; }
        TargetCategory Category { get; }
        bool IsAlive { get; }
    }

    public static class TargetRegistry
    {
        public static readonly List<ITargetable> All = new();
        public static void Register(ITargetable t) { if (!All.Contains(t)) All.Add(t); }
        public static void Unregister(ITargetable t) { All.Remove(t); }
    }

    public class AggroComponent : MonoBehaviour
    {
        public Transform Current { get; private set; }
        public TargetCategory CurrentCategory { get; private set; }

        public void Refresh(Vector3 origin, TargetCategory[] priority, float searchRadius = 80f)
        {
            float sqr = searchRadius * searchRadius;
            Transform best = null; float bestScore = float.MaxValue; TargetCategory bestCat = TargetCategory.Player;
            int prefCount = priority?.Length ?? 0;
            foreach (var t in TargetRegistry.All)
            {
                if (t == null || !t.IsAlive || t.Transform == null) continue;
                float d = (t.Transform.position - origin).sqrMagnitude;
                if (d > sqr) continue;
                int prio = prefCount;
                for (int i = 0; i < prefCount; i++) if (priority[i] == t.Category) { prio = i; break; }
                float score = prio * 1000f + d;
                if (score < bestScore) { bestScore = score; best = t.Transform; bestCat = t.Category; }
            }
            Current = best; CurrentCategory = bestCat;
        }
    }
}
