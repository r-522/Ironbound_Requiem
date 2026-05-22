// 役割: 初期 MVP のオフライン実装。ローカルで完結し、将来 ServerNetworkService に差し替え可能。
using System;
using UnityEngine;

namespace Ironbound.Net
{
    public class OfflineSessionService : INetworkService
    {
        public bool IsConnected => true;
        public event Action<NetMessage> OnEvent;
        private string _session;

        public void StartSession(string sessionId)
        {
            _session = sessionId ?? Guid.NewGuid().ToString("N");
            Debug.Log($"[Offline] Session started: {_session}");
        }

        public void EndSession()
        {
            Debug.Log($"[Offline] Session ended: {_session}");
            _session = null;
        }

        public void SendCommand(NetMessage msg)
        {
            // 即時 echo: 将来サーバ実装ではここで送信し、受信を OnEvent で受け取る。
            OnEvent?.Invoke(msg);
        }
    }
}
