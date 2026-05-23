// 役割: ウェーブ生成点。WaveManager から参照される。
using UnityEngine;

namespace Ironbound.World
{
    public class SpawnPoint : MonoBehaviour
    {
        public int Index;
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0.3f, 0.2f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
    }
}
