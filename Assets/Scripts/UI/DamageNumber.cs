// 役割: 被弾位置に数値を浮かべる軽量ポップ。EventBus を購読し自動生成。
using UnityEngine;
using TMPro;
using Ironbound.Core;

namespace Ironbound.UI
{
    public class DamageNumber : MonoBehaviour
    {
        public TMP_Text Prefab;       // World-space TMP
        public Canvas Canvas;

        private void OnEnable() => EventBus.Subscribe<DamageDealtEvent>(OnDamage);
        private void OnDisable() => EventBus.Unsubscribe<DamageDealtEvent>(OnDamage);

        private void OnDamage(DamageDealtEvent e)
        {
            if (Prefab == null || e.Target == null) return;
            var t = Instantiate(Prefab, Canvas != null ? Canvas.transform : transform);
            t.transform.position = e.Target.transform.position + Vector3.up * 1.8f;
            t.text = Mathf.RoundToInt(e.Amount).ToString();
            Destroy(t.gameObject, 0.9f);
        }
    }
}
