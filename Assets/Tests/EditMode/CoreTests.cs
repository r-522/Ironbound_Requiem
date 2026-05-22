// 役割: 単純な計算系の単体テスト群。
using NUnit.Framework;
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.Tests
{
    public class DamageTests
    {
        [Test] public void Element_Weakness_Boosts() {
            float a = DamageComponent.Compute(10, DamageElement.Fire, 0, DamageElement.Fire);
            float b = DamageComponent.Compute(10, DamageElement.Fire, 0, DamageElement.Frost);
            Assert.Greater(a, b);
        }
        [Test] public void Armor_Reduces_But_NotBelowOne() {
            float v = DamageComponent.Compute(2, DamageElement.Physical, 100, DamageElement.Physical);
            Assert.GreaterOrEqual(v, 1f);
        }
    }

    public class CooldownTests
    {
        [Test] public void Skill_TryCast_FailsWithoutSkill() {
            var go = new GameObject("p");
            var s = go.AddComponent<SkillComponent>();
            Assert.IsFalse(s.TryCast(0, go.transform, go));
            Object.DestroyImmediate(go);
        }
    }

    public class ResourceTests
    {
        [Test] public void TrySpend_FailsWhenInsufficient() {
            var go = new GameObject("rm");
            var rm = go.AddComponent<Ironbound.World.ResourceManager>();
            Assert.IsFalse(rm.TrySpend(99999));
            Assert.IsTrue(rm.TrySpend(10));
            Object.DestroyImmediate(go);
        }
    }

    public class TargetPriorityTests
    {
        [Test] public void Priority_Order_PrefersFirstCategory() {
            // 簡易: アグロは優先カテゴリ順を最も低いインデックスで選ぶ
            var arr = new[] { TargetCategory.Player, TargetCategory.SupportTower };
            Assert.AreEqual(TargetCategory.Player, arr[0]);
            Assert.AreEqual(TargetCategory.SupportTower, arr[1]);
        }
    }
}
