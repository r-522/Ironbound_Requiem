// 役割: タイトル → クラス選択 → ミッション → リザルト の最小状態遷移。
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ironbound.Core
{
    public enum GameState { Title, ClassSelect, Mission, Result }

    public static class GameStateMachine
    {
        public static GameState Current { get; private set; } = GameState.Title;
        public static string SelectedClassId { get; set; } = "Vanguard";

        public static void GoTitle()       { Current = GameState.Title;       SceneManager.LoadScene("Title"); }
        public static void GoClassSelect() { Current = GameState.ClassSelect; SceneManager.LoadScene("ClassSelect"); }
        public static void GoMission()     { Current = GameState.Mission;     SceneManager.LoadScene("AshenPlain"); }
        public static void GoResult()      { Current = GameState.Result;      SceneManager.LoadScene("Result"); }
    }
}
