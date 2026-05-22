// 役割: 建築プレビューの半透明表示と配置可否色。
using UnityEngine;

namespace Ironbound.Towers
{
    public class TowerPreview : MonoBehaviour
    {
        public bool IsPlaceable { get; set; } = true;
        private Renderer[] _renderers;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _block = new MaterialPropertyBlock();
        }

        public void SetTint(Color c)
        {
            if (_renderers == null) return;
            foreach (var r in _renderers)
            {
                if (r == null) continue;
                r.GetPropertyBlock(_block);
                _block.SetColor("_BaseColor", c);
                _block.SetColor("_Color", c);
                r.SetPropertyBlock(_block);
            }
        }
    }
}
