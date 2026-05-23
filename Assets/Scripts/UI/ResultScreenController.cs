// 役割: 勝敗結果の表示と再挑戦/タイトルへの遷移。
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironbound.Core;

namespace Ironbound.UI
{
    public class ResultScreenController : MonoBehaviour
    {
        public TMP_Text HeadlineLabel;
        public TMP_Text DetailLabel;
        public Button RetryButton;
        public Button TitleButton;

        public void Show(bool victory, float duration, int score)
        {
            if (HeadlineLabel != null) HeadlineLabel.text = victory ? "VICTORY" : "FALLEN";
            if (DetailLabel != null) DetailLabel.text = $"TIME {Mathf.FloorToInt(duration / 60f):00}:{Mathf.FloorToInt(duration % 60f):00}    SCORE {score}";
        }

        private void Start()
        {
            if (RetryButton != null) RetryButton.onClick.AddListener(GameStateMachine.GoMission);
            if (TitleButton != null) TitleButton.onClick.AddListener(GameStateMachine.GoTitle);
        }
    }
}
