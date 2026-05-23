// 役割: HUD 表示。HP/Stamina/Mana/スキルCD/資源/フェーズ/Wave 時間/補給線。
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironbound.Combat;
using Ironbound.Core;
using Ironbound.World;

namespace Ironbound.UI
{
    public class UIHudController : MonoBehaviour
    {
        [Header("Bars")]
        public Slider HpBar;
        public Slider StaminaBar;
        public Slider ManaBar;
        public Image[] SkillCdMasks = new Image[4];
        public TMP_Text ResourceText;
        public TMP_Text PhaseText;
        public TMP_Text WaveText;
        public TMP_Text ObjectiveText;
        public Slider SupplyBar;

        [Header("Refs")]
        public HealthComponent PlayerHealth;
        public StaminaComponent PlayerStamina;
        public ManaComponent PlayerMana;
        public SkillComponent PlayerSkills;
        public MissionManager Mission;
        public WaveManager Wave;
        public SupplyLine Supply;

        private void OnEnable()
        {
            EventBus.Subscribe<ResourceChangedEvent>(OnResource);
            EventBus.Subscribe<PhaseChangedEvent>(OnPhase);
            EventBus.Subscribe<WaveStartedEvent>(OnWaveStarted);
            EventBus.Subscribe<WaveClearedEvent>(OnWaveCleared);
            if (PlayerSkills != null) PlayerSkills.OnCooldownChanged += OnSkillCd;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ResourceChangedEvent>(OnResource);
            EventBus.Unsubscribe<PhaseChangedEvent>(OnPhase);
            EventBus.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
            EventBus.Unsubscribe<WaveClearedEvent>(OnWaveCleared);
            if (PlayerSkills != null) PlayerSkills.OnCooldownChanged -= OnSkillCd;
        }

        private void Update()
        {
            if (PlayerHealth != null && HpBar != null) HpBar.value = PlayerHealth.Normalized;
            if (PlayerStamina != null && StaminaBar != null) StaminaBar.value = PlayerStamina.Normalized;
            if (PlayerMana != null && ManaBar != null) ManaBar.value = PlayerMana.Normalized;
            if (Supply != null && SupplyBar != null)
            {
                var hp = Supply.GetComponent<HealthComponent>();
                if (hp != null) SupplyBar.value = hp.Normalized;
            }
            if (Wave != null && WaveText != null)
                WaveText.text = Wave.IsRunning ? $"WAVE {Wave.CurrentWaveIndex} 残敵 {Wave.Active}" : "—";
        }

        private void OnResource(ResourceChangedEvent e) { if (ResourceText != null) ResourceText.text = $"⚙ {e.Current}"; }
        private void OnPhase(PhaseChangedEvent e)      { if (PhaseText != null) PhaseText.text = $"PHASE {e.PhaseIndex + 1} — {e.PhaseName}"; }
        private void OnWaveStarted(WaveStartedEvent e) { if (ObjectiveText != null) ObjectiveText.text = "敵襲来 — 防衛せよ"; }
        private void OnWaveCleared(WaveClearedEvent e) { if (ObjectiveText != null) ObjectiveText.text = "ウェーブ撃退 — 進撃せよ"; }

        private void OnSkillCd(int slot, float remaining, float total)
        {
            if (slot < 0 || slot >= SkillCdMasks.Length) return;
            var m = SkillCdMasks[slot]; if (m == null) return;
            m.fillAmount = total <= 0 ? 0 : remaining / total;
        }
    }
}
