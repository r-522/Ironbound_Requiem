// 役割: 建築モード切替時にホットバーを表示し、選択状態を反映。
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironbound.Towers;

namespace Ironbound.UI
{
    public class BuildMenuController : MonoBehaviour
    {
        public TowerBuildController Build;
        public GameObject Panel;
        public Image[] SlotIcons = new Image[4];
        public TMP_Text[] SlotLabels = new TMP_Text[4];
        public Image[] SlotFrames = new Image[4];
        public Color SelectedColor = new Color(1f, 0.85f, 0.4f);
        public Color NormalColor = new Color(0.6f, 0.55f, 0.45f);
        private int _lastSelected = -1;

        private void Update()
        {
            if (Build == null || Panel == null) return;
            Panel.SetActive(Build.IsActive);
            if (!Build.IsActive) return;
            for (int i = 0; i < SlotIcons.Length; i++)
            {
                var d = Build.Hotbar[i];
                if (SlotLabels[i] != null) SlotLabels[i].text = d != null ? $"{(i + 1)}. {d.Name}\n{d.Cost}⚙" : $"{(i + 1)}.";
                if (SlotIcons[i] != null && d != null) SlotIcons[i].color = d.UiTint;
            }
            // 選択中をハイライト
            int sel = -1;
            for (int i = 0; i < 4; i++) if (Build.Hotbar[i] == Build.Current) { sel = i; break; }
            if (sel != _lastSelected)
            {
                _lastSelected = sel;
                for (int i = 0; i < SlotFrames.Length; i++)
                    if (SlotFrames[i] != null) SlotFrames[i].color = i == sel ? SelectedColor : NormalColor;
            }
        }
    }
}
