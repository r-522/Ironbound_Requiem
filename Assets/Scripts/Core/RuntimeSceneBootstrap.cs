// 役割: シーン名から対応 UI を自動生成する。.unity ファイルなしでも最低限のフローが動くようにする。
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Ironbound.UI;
using Ironbound.Data;
using Ironbound.World;

namespace Ironbound.Core
{
    public static class RuntimeSceneBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoad()
        {
            var scene = SceneManager.GetActiveScene();
            switch (scene.name)
            {
                case "Title":       BuildTitle();      break;
                case "ClassSelect": BuildClassSelect(); break;
                case "AshenPlain":  BuildMission();    break;
                case "Result":      BuildResult();     break;
                default:
                    // 開発中フォールバック: 起動シーン名が異なれば Title 相当を構築
                    if (Object.FindAnyObjectByType<TitleScreenController>() == null
                        && Object.FindAnyObjectByType<SceneFabricator>() == null
                        && Object.FindAnyObjectByType<ClassSelectController>() == null)
                        BuildTitle();
                    break;
            }
        }

        private static Canvas MakeCanvas(string name)
        {
            var go = new GameObject(name);
            var c = go.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<GraphicRaycaster>();
            var bg = new GameObject("BG"); bg.transform.SetParent(go.transform, false);
            var bgRT = bg.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;
            var bgImg = bg.AddComponent<Image>(); bgImg.color = new Color(0.04f, 0.035f, 0.03f, 1f);
            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            return c;
        }

        private static TMP_Text Label(Transform p, string text, Vector2 pos, int size, Color col, TextAlignmentOptions a = TextAlignmentOptions.Center)
        {
            var go = new GameObject("Label"); go.transform.SetParent(p, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(900, 60);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text; t.fontSize = size; t.color = col; t.alignment = a;
            return t;
        }

        private static Button MakeButton(Transform p, string text, Vector2 pos)
        {
            var go = new GameObject("Btn"); go.transform.SetParent(p, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(360, 64);
            var img = go.AddComponent<Image>(); img.color = new Color(0.12f, 0.1f, 0.08f, 1f);
            var btn = go.AddComponent<Button>();
            var colors = btn.colors; colors.highlightedColor = new Color(0.22f, 0.18f, 0.13f); colors.pressedColor = new Color(0.3f, 0.22f, 0.12f); btn.colors = colors;
            var lbl = Label(go.transform, text, Vector2.zero, 22, new Color(0.92f, 0.84f, 0.6f));
            lbl.alignment = TextAlignmentOptions.Center;
            return btn;
        }

        private static void BuildTitle()
        {
            var canvas = MakeCanvas("TitleCanvas");
            Label(canvas.transform, "CROWN OF ASHVALD",      new Vector2(0,  140), 56, new Color(0.85f, 0.7f, 0.35f));
            Label(canvas.transform, "IRONBOUND  REQUIEM",    new Vector2(0,  80), 24, new Color(0.55f, 0.45f, 0.25f));
            Label(canvas.transform, "— Vertical Slice MVP —", new Vector2(0,  40), 16, new Color(0.45f, 0.4f, 0.3f));
            var ctrl = canvas.gameObject.AddComponent<TitleScreenController>();
            ctrl.TitleLabel = canvas.transform.Find("Label")?.GetComponent<TMP_Text>();
            ctrl.StartButton = MakeButton(canvas.transform, "▷  START EXPEDITION", new Vector2(0, -40));
            ctrl.QuitButton  = MakeButton(canvas.transform, "EXIT",                new Vector2(0, -120));
            ctrl.StartButton.onClick.AddListener(GameStateMachine.GoClassSelect);
            ctrl.QuitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        private static void BuildClassSelect()
        {
            var canvas = MakeCanvas("ClassSelectCanvas");
            Label(canvas.transform, "CHOOSE YOUR PATH", new Vector2(0, 220), 36, new Color(0.85f, 0.7f, 0.35f));
            var ctrl = canvas.gameObject.AddComponent<ClassSelectController>();
            ctrl.Classes = DefaultDataFactory.BuildClasses();
            ctrl.ClassButtons = new Button[4];
            string[] names = { "Vanguard", "Arcanist", "Warden", "Ranger" };
            for (int i = 0; i < 4; i++) ctrl.ClassButtons[i] = MakeButton(canvas.transform, names[i], new Vector2((i - 1.5f) * 200, 80));
            ctrl.NameLabel = Label(canvas.transform, "Vanguard", new Vector2(0, -40), 28, new Color(0.85f, 0.7f, 0.35f));
            ctrl.DescriptionLabel = Label(canvas.transform, "", new Vector2(0, -100), 18, new Color(0.8f, 0.75f, 0.6f));
            ctrl.ConfirmButton = MakeButton(canvas.transform, "▷  CONFIRM", new Vector2(0, -200));
        }

        private static void BuildMission()
        {
            if (Object.FindAnyObjectByType<SceneFabricator>() != null) return;
            var go = new GameObject("SceneFabricator");
            var fab = go.AddComponent<SceneFabricator>();
            fab.AvailableClasses = DefaultDataFactory.BuildClasses();
            var towers = DefaultDataFactory.BuildTowers();
            fab.StarterHotbar = new[] { towers[0], towers[3], towers[7], towers[11] }; // 壁/Arrow/Heal/TrapMine
            var enemies = DefaultDataFactory.BuildEnemies();
            fab.Mission = DefaultDataFactory.BuildMission(enemies);
        }

        private static void BuildResult()
        {
            var canvas = MakeCanvas("ResultCanvas");
            Label(canvas.transform, "VICTORY", new Vector2(0, 120), 64, new Color(0.92f, 0.78f, 0.4f));
            Label(canvas.transform, "前線は押し戻された。次なる地平へ。", new Vector2(0, 40), 22, new Color(0.8f, 0.72f, 0.55f));
            var ctrl = canvas.gameObject.AddComponent<ResultScreenController>();
            ctrl.RetryButton = MakeButton(canvas.transform, "▷  REDEPLOY", new Vector2(0, -60));
            ctrl.TitleButton = MakeButton(canvas.transform, "TO TITLE", new Vector2(0, -140));
        }
    }
}
