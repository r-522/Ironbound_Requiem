// 役割: タワー発射弾の生成。プロジェクタイル prefab があれば使い、なければ Sphere 代替。
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Data;

namespace Ironbound.Towers
{
    public static class TowerProjectile
    {
        public static void Fire(Vector3 origin, Transform target, TowerData data, GameObject owner)
        {
            GameObject go = data.ProjectilePrefab != null
                ? Object.Instantiate(data.ProjectilePrefab)
                : GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "TowerProjectile_" + data.TowerId;
            go.transform.localScale = Vector3.one * 0.25f;
            go.transform.position = origin;
            foreach (var c in go.GetComponents<Collider>()) c.isTrigger = true;
            var rend = go.GetComponent<Renderer>();
            if (rend != null && rend.material.HasProperty("_BaseColor")) rend.material.SetColor("_BaseColor", data.UiTint);
            var p = go.AddComponent<ProjectileComponent>();
            p.Damage = data.Damage; p.Element = data.Element; p.Owner = owner;
            p.Launch((target.position + Vector3.up * 0.8f - origin).normalized);
        }
    }
}
