// 役割: 被弾リアクション(被弾色のフラッシュ + 軽い硬直)。部位別拡張ポイント。
using System.Collections;
using UnityEngine;

namespace Ironbound.Combat
{
    public class HitReactionComponent : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color flashColor = new Color(1f, 0.4f, 0.3f, 1f);
        [SerializeField] private float flashTime = 0.08f;
        public float StaggerTime { get; private set; }

        private void Reset()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        public void Trigger(Vector3 hitDir, float stagger = 0.15f)
        {
            StaggerTime = stagger;
            StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            var matsOrig = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;
                if (renderers[i].material.HasProperty("_BaseColor"))
                {
                    matsOrig[i] = renderers[i].material.GetColor("_BaseColor");
                    renderers[i].material.SetColor("_BaseColor", flashColor);
                }
                else if (renderers[i].material.HasProperty("_Color"))
                {
                    matsOrig[i] = renderers[i].material.color;
                    renderers[i].material.color = flashColor;
                }
            }
            yield return new WaitForSeconds(flashTime);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;
                if (renderers[i].material.HasProperty("_BaseColor")) renderers[i].material.SetColor("_BaseColor", matsOrig[i]);
                else if (renderers[i].material.HasProperty("_Color")) renderers[i].material.color = matsOrig[i];
            }
        }

        private void Update()
        {
            if (StaggerTime > 0) StaggerTime -= Time.deltaTime;
        }
    }
}
