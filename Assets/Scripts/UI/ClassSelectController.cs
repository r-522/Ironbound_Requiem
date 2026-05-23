// 役割: 4 クラスを表示し、選択結果を GameStateMachine.SelectedClassId に保存。
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironbound.Core;
using Ironbound.Data;

namespace Ironbound.UI
{
    public class ClassSelectController : MonoBehaviour
    {
        public PlayerClassData[] Classes;
        public Button[] ClassButtons;       // 4 個
        public TMP_Text NameLabel;
        public TMP_Text DescriptionLabel;
        public Button ConfirmButton;
        private int _selected;

        private void Start()
        {
            for (int i = 0; i < ClassButtons.Length; i++)
            {
                int idx = i;
                ClassButtons[i].onClick.AddListener(() => Select(idx));
                var label = ClassButtons[i].GetComponentInChildren<TMP_Text>();
                if (label != null && idx < Classes.Length && Classes[idx] != null) label.text = Classes[idx].DisplayName;
            }
            Select(0);
            if (ConfirmButton != null) ConfirmButton.onClick.AddListener(Confirm);
        }

        private void Select(int idx)
        {
            _selected = Mathf.Clamp(idx, 0, Classes.Length - 1);
            var c = Classes[_selected];
            if (NameLabel != null) NameLabel.text = c.DisplayName;
            if (DescriptionLabel != null) DescriptionLabel.text = c.Description;
        }

        private void Confirm()
        {
            GameStateMachine.SelectedClassId = Classes[_selected].ClassId;
            GameStateMachine.GoMission();
        }
    }
}
