// 役割: 音量・マウス感度等の最小設定。PlayerPrefs に保存。
using UnityEngine;
using UnityEngine.UI;
using Ironbound.Audio;

namespace Ironbound.UI
{
    public class SettingsController : MonoBehaviour
    {
        public Slider MasterSlider;
        public Slider SfxSlider;
        public Slider BgmSlider;

        private void Start()
        {
            if (MasterSlider != null) { MasterSlider.value = PlayerPrefs.GetFloat("vol_master", 1f); MasterSlider.onValueChanged.AddListener(v => { AudioManager.Instance.MasterVolume = v; PlayerPrefs.SetFloat("vol_master", v); }); }
            if (SfxSlider != null) { SfxSlider.value = PlayerPrefs.GetFloat("vol_sfx", 1f); SfxSlider.onValueChanged.AddListener(v => { AudioManager.Instance.SfxVolume = v; PlayerPrefs.SetFloat("vol_sfx", v); }); }
            if (BgmSlider != null) { BgmSlider.value = PlayerPrefs.GetFloat("vol_bgm", 0.7f); BgmSlider.onValueChanged.AddListener(v => { AudioManager.Instance.BgmVolume = v; PlayerPrefs.SetFloat("vol_bgm", v); }); }
        }
    }
}
