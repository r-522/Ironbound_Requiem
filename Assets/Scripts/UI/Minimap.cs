// 役割: 簡易レーダー。プレイヤー中心に敵/タワーをドットで描画する RawImage を直接描く。
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ironbound.AI;

namespace Ironbound.UI
{
    public class Minimap : MonoBehaviour
    {
        public Transform Player;
        public RawImage Canvas;
        public int Size = 128;
        public float Range = 40f;
        private Texture2D _tex;
        private Color[] _clear;

        private void Start()
        {
            _tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            _tex.filterMode = FilterMode.Point;
            _clear = new Color[Size * Size];
            for (int i = 0; i < _clear.Length; i++) _clear[i] = new Color(0, 0, 0, 0.35f);
            if (Canvas != null) Canvas.texture = _tex;
        }

        private void LateUpdate()
        {
            if (_tex == null || Player == null) return;
            _tex.SetPixels(_clear);
            // 中心点
            PutPixel(Size / 2, Size / 2, Color.white);
            // 敵
            foreach (var e in Object.FindObjectsByType<EnemyAIController>(FindObjectsSortMode.None))
            {
                if (e == null) continue;
                MapAndPut(e.transform.position, new Color(1f, 0.3f, 0.25f));
            }
            // タワー
            foreach (var t in TargetRegistry.All)
            {
                if (t == null || t.Transform == null) continue;
                Color c = t.Category switch
                {
                    Ironbound.Data.TargetCategory.SupportTower => new Color(0.4f, 0.9f, 1f),
                    Ironbound.Data.TargetCategory.Tower => new Color(1f, 0.85f, 0.4f),
                    Ironbound.Data.TargetCategory.Barricade => new Color(0.6f, 0.6f, 0.55f),
                    Ironbound.Data.TargetCategory.SupplyLine => new Color(0.95f, 0.6f, 0.2f),
                    _ => Color.clear
                };
                if (c == Color.clear) continue;
                MapAndPut(t.Transform.position, c);
            }
            _tex.Apply(false);
        }

        private void MapAndPut(Vector3 worldPos, Color c)
        {
            Vector3 d = worldPos - Player.position;
            int px = Mathf.RoundToInt(Size / 2 + (d.x / Range) * (Size / 2));
            int py = Mathf.RoundToInt(Size / 2 + (d.z / Range) * (Size / 2));
            PutPixel(px, py, c);
            PutPixel(px + 1, py, c); PutPixel(px, py + 1, c);
        }

        private void PutPixel(int x, int y, Color c)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size) return;
            _tex.SetPixel(x, y, c);
        }
    }
}
