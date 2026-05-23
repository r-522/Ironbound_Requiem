// 役割: ゲームロジックと通信実装を分離する抽象インターフェース。
using System;

namespace Ironbound.Net
{
    public enum NetCommand { StartMission, EndMission, BuildTower, SpendResource, DealDamage }

    public struct NetMessage
    {
        public NetCommand Command;
        public string Payload;
        public int IntValue;
        public float FloatValue;
    }

    public interface INetworkService
    {
        bool IsConnected { get; }
        event Action<NetMessage> OnEvent;
        void StartSession(string sessionId);
        void EndSession();
        void SendCommand(NetMessage msg);
    }
}
