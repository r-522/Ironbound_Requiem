// 役割: シーンに最低限の地形・スポーン点・補給線・プレイヤー・カメラ・HUD・ミッションを動的構築。
// 実機アセットが揃うまでのプレイアブル雛形として機能する。
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.UI;
using TMPro;
using Ironbound.AI;
using Ironbound.Audio;
using Ironbound.Combat;
using Ironbound.Core;
using Ironbound.Data;
using Ironbound.Player;
using Ironbound.Towers;
using Ironbound.UI;

namespace Ironbound.World
{
    public class SceneFabricator : MonoBehaviour
    {
        public MissionData Mission;
        public PlayerClassData[] AvailableClasses;
        public TowerData[] StarterHotbar = new TowerData[4];
        public AudioClip AmbientBgm;

        private void Awake()
        {
            BuildEnvironment();
            var player = BuildPlayer();
            var mission = BuildMissionRig(player);
            var hud = BuildHud(player, mission);
            if (AudioManager.Instance != null && AmbientBgm != null) AudioManager.Instance.PlayBgm(AmbientBgm);
        }

        private void BuildEnvironment()
        {
            // 地面
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "AshenPlain_Ground";
            ground.transform.localScale = Vector3.one * 12f;
            var groundRend = ground.GetComponent<Renderer>();
            if (groundRend != null && groundRend.material.HasProperty("_BaseColor"))
                groundRend.material.SetColor("_BaseColor", new Color(0.18f, 0.16f, 0.14f));
            ground.layer = 0;
            // NavMesh
            var surf = ground.AddComponent<NavMeshSurface>();
            surf.collectObjects = CollectObjects.All;
            surf.BuildNavMesh();

            // ライト
            var lightGO = new GameObject("KeyLight");
            var l = lightGO.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = new Color(1f, 0.85f, 0.65f);
            l.intensity = 1.1f;
            lightGO.transform.rotation = Quaternion.Euler(48f, 35f, 0);
            RenderSettings.ambientLight = new Color(0.18f, 0.16f, 0.18f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.12f, 0.12f, 0.14f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.012f;

            // スポーン地点 x3
            for (int i = 0; i < 3; i++)
            {
                var sp = new GameObject("SpawnPoint_" + i).AddComponent<SpawnPoint>();
                sp.Index = i;
                float ang = i * Mathf.PI * 2f / 3f;
                sp.transform.position = new Vector3(Mathf.Cos(ang) * 28f, 0, Mathf.Sin(ang) * 28f);
            }

            // 補給線
            var supplyGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            supplyGo.name = "SupplyLine";
            supplyGo.transform.position = new Vector3(0, 1, -8);
            supplyGo.transform.localScale = new Vector3(3, 2, 3);
            supplyGo.AddComponent<HealthComponent>();
            supplyGo.AddComponent<SupplyLine>();
            var rend = supplyGo.GetComponent<Renderer>();
            if (rend != null && rend.material.HasProperty("_BaseColor")) rend.material.SetColor("_BaseColor", new Color(0.35f, 0.25f, 0.15f));

            // ボススポーン地点
            new GameObject("BossSpawn").transform.position = new Vector3(0, 0, 18);
        }

        private GameObject BuildPlayer()
        {
            var go = new GameObject("Player");
            go.transform.position = new Vector3(0, 1f, 0);
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.8f; cc.radius = 0.4f; cc.center = new Vector3(0, 0.9f, 0);
            // 見た目
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(go.transform, false);
            body.transform.localPosition = new Vector3(0, 0.9f, 0);
            Destroy(body.GetComponent<Collider>());
            var rend = body.GetComponent<Renderer>();
            if (rend != null && rend.material.HasProperty("_BaseColor")) rend.material.SetColor("_BaseColor", new Color(0.35f, 0.35f, 0.4f));

            go.AddComponent<PlayerInput>();
            go.AddComponent<HealthComponent>();
            go.AddComponent<StaminaComponent>();
            go.AddComponent<ManaComponent>();
            go.AddComponent<ComboController>();
            go.AddComponent<SkillComponent>();
            go.AddComponent<DodgeComponent>();
            go.AddComponent<GuardComponent>();
            go.AddComponent<KnockbackComponent>();
            go.AddComponent<HitReactionComponent>();
            var loadout = go.AddComponent<ClassLoadout>();
            go.AddComponent<PlayerTargetTag>();
            var lockOn = go.AddComponent<LockOnController>();
            var build = go.AddComponent<TowerBuildController>();
            build.Hotbar = StarterHotbar;
            var ctrl = go.AddComponent<PlayerController>();

            // クラス適用
            PlayerClassData selected = null;
            if (AvailableClasses != null)
            {
                foreach (var c in AvailableClasses)
                    if (c != null && c.ClassId == GameStateMachine.SelectedClassId) { selected = c; break; }
                if (selected == null && AvailableClasses.Length > 0) selected = AvailableClasses[0];
            }
            loadout.Apply(selected);

            // カメラ
            var camGo = new GameObject("MainCamera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 60f;
            camGo.AddComponent<AudioListener>();
            var rig = camGo.AddComponent<ThirdPersonCameraController>();
            rig.Target = go.transform;
            camGo.AddComponent<CameraShakeController>();
            HitStopService.Ensure();
            return go;
        }

        private MissionManager BuildMissionRig(GameObject player)
        {
            var go = new GameObject("Mission");
            var res = go.AddComponent<ResourceManager>();
            var wave = go.AddComponent<WaveManager>();
            // SpawnPoints と Player を WaveManager に渡す
            var spField = typeof(WaveManager).GetField("spawnPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            spField?.SetValue(wave, Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None));
            var pField = typeof(WaveManager).GetField("player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pField?.SetValue(wave, player.transform);

            var mission = go.AddComponent<MissionManager>();
            mission.Mission = Mission;
            var mField = typeof(MissionManager).GetField("wave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            mField?.SetValue(mission, wave);
            var bField = typeof(MissionManager).GetField("bossSpawn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bossSpawn = GameObject.Find("BossSpawn");
            if (bossSpawn != null) bField?.SetValue(mission, bossSpawn.transform);
            var pField2 = typeof(MissionManager).GetField("player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pField2?.SetValue(mission, player.transform);
            return mission;
        }

        private GameObject BuildHud(GameObject player, MissionManager mission)
        {
            var canvasGo = new GameObject("HUDCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();
            var hud = canvasGo.AddComponent<UIHudController>();
            hud.PlayerHealth = player.GetComponent<HealthComponent>();
            hud.PlayerStamina = player.GetComponent<StaminaComponent>();
            hud.PlayerMana = player.GetComponent<ManaComponent>();
            hud.PlayerSkills = player.GetComponent<SkillComponent>();
            hud.Mission = mission;
            hud.Wave = mission.GetComponent<WaveManager>();
            hud.Supply = Object.FindAnyObjectByType<SupplyLine>();

            hud.HpBar       = MakeBar(canvasGo.transform, new Vector2(20,  -20), new Color(0.7f, 0.2f, 0.2f));
            hud.StaminaBar  = MakeBar(canvasGo.transform, new Vector2(20,  -45), new Color(0.85f, 0.75f, 0.3f));
            hud.ManaBar     = MakeBar(canvasGo.transform, new Vector2(20,  -70), new Color(0.3f, 0.55f, 0.95f));
            hud.SupplyBar   = MakeBar(canvasGo.transform, new Vector2(20,  -95), new Color(0.9f, 0.55f, 0.2f));
            hud.ResourceText  = MakeText(canvasGo.transform, new Vector2(20, -120), "⚙ 80");
            hud.PhaseText     = MakeText(canvasGo.transform, new Vector2(0,  -20), "PHASE 1", TextAlignmentOptions.Top, true);
            hud.WaveText      = MakeText(canvasGo.transform, new Vector2(0,  -50), "—",       TextAlignmentOptions.Top, true);
            hud.ObjectiveText = MakeText(canvasGo.transform, new Vector2(0,  -80), "前線を押し上げよ", TextAlignmentOptions.Top, true);

            // 建築 UI
            var build = player.GetComponent<TowerBuildController>();
            var bmGo = new GameObject("BuildMenu"); bmGo.transform.SetParent(canvasGo.transform, false);
            var bm = bmGo.AddComponent<BuildMenuController>();
            bm.Build = build; bm.Panel = bmGo;
            for (int i = 0; i < 4; i++)
            {
                var slot = new GameObject("BSlot" + i);
                slot.transform.SetParent(bmGo.transform, false);
                var rect = slot.AddComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.1f);
                rect.anchoredPosition = new Vector2((i - 1.5f) * 96, 0);
                rect.sizeDelta = new Vector2(88, 88);
                var frame = slot.AddComponent<Image>(); frame.color = new Color(0.6f, 0.55f, 0.45f); bm.SlotFrames[i] = frame;
                var iconGo = new GameObject("Icon"); iconGo.transform.SetParent(slot.transform, false);
                var iconRT = iconGo.AddComponent<RectTransform>(); iconRT.anchorMin = Vector2.zero; iconRT.anchorMax = Vector2.one; iconRT.offsetMin = new Vector2(4, 4); iconRT.offsetMax = new Vector2(-4, -28);
                bm.SlotIcons[i] = iconGo.AddComponent<Image>();
                var labelGo = new GameObject("Label"); labelGo.transform.SetParent(slot.transform, false);
                var labelRT = labelGo.AddComponent<RectTransform>(); labelRT.anchorMin = new Vector2(0, 0); labelRT.anchorMax = new Vector2(1, 0); labelRT.offsetMin = new Vector2(2, 2); labelRT.offsetMax = new Vector2(-2, 26);
                var t = labelGo.AddComponent<TextMeshProUGUI>();
                t.fontSize = 14; t.alignment = TextAlignmentOptions.Center; t.color = new Color(0.92f, 0.88f, 0.75f);
                bm.SlotLabels[i] = t;
            }
            bmGo.SetActive(false);
            return canvasGo;
        }

        private static Slider MakeBar(Transform parent, Vector2 anchored, Color color)
        {
            var go = new GameObject("Bar");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(220, 18);
            rt.anchoredPosition = anchored;
            var bg = go.AddComponent<Image>(); bg.color = new Color(0, 0, 0, 0.55f);
            var fillGo = new GameObject("Fill"); fillGo.transform.SetParent(go.transform, false);
            var fillRT = fillGo.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = new Vector2(2, 2); fillRT.offsetMax = new Vector2(-2, -2);
            var fillImg = fillGo.AddComponent<Image>(); fillImg.color = color;
            var slider = go.AddComponent<Slider>();
            slider.targetGraphic = fillImg; slider.fillRect = fillRT; slider.minValue = 0; slider.maxValue = 1; slider.value = 1;
            return slider;
        }

        private static TMP_Text MakeText(Transform parent, Vector2 anchored, string text,
                                         TextAlignmentOptions align = TextAlignmentOptions.TopLeft, bool topCenter = false)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            if (topCenter) { rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f); rt.pivot = new Vector2(0.5f, 1f); }
            else { rt.anchorMin = rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0, 1); }
            rt.anchoredPosition = anchored;
            rt.sizeDelta = new Vector2(420, 28);
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text; t.fontSize = 18; t.alignment = align; t.color = new Color(0.92f, 0.88f, 0.75f);
            return t;
        }
    }
}
