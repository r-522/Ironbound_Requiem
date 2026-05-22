// 役割: アプリ起動時に共通サービスを初期化し、シーン遷移基盤を整える。
using UnityEngine;
using UnityEngine.SceneManagement;
using Ironbound.Net;
using Ironbound.Audio;

namespace Ironbound.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string titleScene = "Title";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootstrapServices()
        {
            ServiceLocator.Clear();
            ServiceLocator.Register<INetworkService>(new OfflineSessionService());
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (AudioManager.Instance == null)
                new GameObject("AudioManager").AddComponent<AudioManager>();
        }

        public static void LoadScene(string scene) => SceneManager.LoadScene(scene);
    }
}
