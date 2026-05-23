// 役割: 建築モード制御。プレビュー表示、配置可否、資源消費、生成。
using UnityEngine;
using UnityEngine.AI;
using Ironbound.Data;
using Ironbound.World;

namespace Ironbound.Towers
{
    public class TowerBuildController : MonoBehaviour
    {
        public TowerData[] Hotbar = new TowerData[4];
        public bool IsActive { get; private set; }
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private float maxBuildDistance = 12f;
        private GameObject _preview;
        private int _slot;
        private TowerPreview _previewScript;

        public void SetActive(bool on)
        {
            IsActive = on;
            if (!on) Cleanup();
            else BuildPreview();
        }

        public void SelectSlot(int slot)
        {
            _slot = Mathf.Clamp(slot, 0, Hotbar.Length - 1);
            BuildPreview();
        }

        private void BuildPreview()
        {
            Cleanup();
            var data = Current;
            if (data == null) return;
            _preview = data.Prefab != null ? Instantiate(data.Prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
            _preview.name = "Preview_" + data.TowerId;
            foreach (var col in _preview.GetComponentsInChildren<Collider>()) col.enabled = false;
            _previewScript = _preview.AddComponent<TowerPreview>();
            _previewScript.SetTint(new Color(0.4f, 1f, 0.4f, 0.6f));
        }

        public TowerData Current => Hotbar[_slot];

        public bool ConfirmBuild()
        {
            if (!IsActive || _preview == null || Current == null) return false;
            if (!_previewScript.IsPlaceable) return false;
            var res = ResourceManager.Instance;
            if (res == null || !res.TrySpend(Current.Cost)) return false;
            var go = Current.Prefab != null ? Instantiate(Current.Prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = Current.TowerId;
            go.transform.position = _preview.transform.position;
            go.transform.rotation = _preview.transform.rotation;
            if (Current.Category == TowerCategory.Wall || Current.Category == TowerCategory.Special)
            {
                if (go.GetComponent<NavMeshObstacle>() == null)
                {
                    var ob = go.AddComponent<NavMeshObstacle>();
                    ob.carving = true; ob.size = Vector3.one * 1.1f;
                }
            }
            var tc = go.GetComponent<TowerComponent>() ?? go.AddComponent<TowerComponent>();
            tc.Initialize(Current);
            return true;
        }

        private void Cleanup()
        {
            if (_preview != null) Destroy(_preview);
            _preview = null; _previewScript = null;
        }

        private void Update()
        {
            if (!IsActive || _preview == null) return;
            if (cam == null) cam = Camera.main;
            if (cam == null) return;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, maxBuildDistance + 30f, groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 p = hit.point;
                p.x = Mathf.Round(p.x / gridSize) * gridSize;
                p.z = Mathf.Round(p.z / gridSize) * gridSize;
                _preview.transform.position = p;
                bool tooFar = Vector3.Distance(p, transform.position) > maxBuildDistance;
                _previewScript.IsPlaceable = !tooFar;
                _previewScript.SetTint(tooFar ? new Color(1, 0.3f, 0.3f, 0.6f) : new Color(0.4f, 1f, 0.4f, 0.6f));
            }
        }
    }
}
