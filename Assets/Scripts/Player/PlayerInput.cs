// 役割: 旧 Input Manager (キーボード+マウス) を元に毎フレームの入力を集約する。
using UnityEngine;

namespace Ironbound.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool JumpPressed;
        public bool DashHeld;
        public bool CrouchHeld;
        public bool InteractPressed;
        public bool BasicAttackPressed;
        public bool HeavyAttackPressed;
        public bool GuardHeld;       // RMB 長押し → ガード
        public bool DodgePressed;
        public bool Skill1Pressed, Skill2Pressed, Skill3Pressed, UltimatePressed;
        public bool BuildModeToggled;
        public int TowerSlot;        // 1..4 (0=未選択)
        public bool LockOnPressed;
        public bool MenuPressed;

        private void Update()
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            JumpPressed = Input.GetKeyDown(KeyCode.Space);
            DashHeld = Input.GetKey(KeyCode.LeftShift);
            CrouchHeld = Input.GetKey(KeyCode.LeftControl);
            InteractPressed = Input.GetKeyDown(KeyCode.E);
            BasicAttackPressed = Input.GetMouseButtonDown(0);
            HeavyAttackPressed = Input.GetMouseButtonDown(1);
            GuardHeld = Input.GetMouseButton(1);
            DodgePressed = Input.GetKeyDown(KeyCode.LeftAlt);
            Skill1Pressed = Input.GetKeyDown(KeyCode.Q);
            Skill2Pressed = Input.GetKeyDown(KeyCode.C);
            Skill3Pressed = Input.GetKeyDown(KeyCode.V);
            UltimatePressed = Input.GetKeyDown(KeyCode.Z);
            BuildModeToggled = Input.GetKeyDown(KeyCode.B);
            TowerSlot = 0;
            if (Input.GetKeyDown(KeyCode.Alpha1)) TowerSlot = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) TowerSlot = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) TowerSlot = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) TowerSlot = 4;
            LockOnPressed = Input.GetMouseButtonDown(2);
            MenuPressed = Input.GetKeyDown(KeyCode.Escape);
        }
    }
}
