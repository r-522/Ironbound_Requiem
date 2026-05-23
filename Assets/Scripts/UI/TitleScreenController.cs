// 役割: タイトル画面。STARTでクラス選択へ。
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironbound.Core;

namespace Ironbound.UI
{
    public class TitleScreenController : MonoBehaviour
    {
        public Button StartButton;
        public Button QuitButton;
        public TMP_Text TitleLabel;
        public TMP_Text Subtitle;

        private void Start()
        {
            if (TitleLabel != null) TitleLabel.text = "CROWN OF ASHVALD";
            if (Subtitle != null) Subtitle.text = "IRONBOUND REQUIEM — VERTICAL SLICE";
            if (StartButton != null) StartButton.onClick.AddListener(GameStateMachine.GoClassSelect);
            if (QuitButton != null) QuitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }
}
