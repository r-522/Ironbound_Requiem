// 役割: 入力に応じて移動・ジャンプ・回避・攻撃・スキル発動を統括するプレイヤー本体。
using UnityEngine;
using Ironbound.Combat;
using Ironbound.Core;
using Ironbound.Towers;

namespace Ironbound.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5.5f;
        [SerializeField] private float runMultiplier = 1.55f;
        [SerializeField] private float jumpSpeed = 6.5f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float turnLerp = 14f;

        [Header("Refs")]
        [SerializeField] private PlayerInput input;
        [SerializeField] private ThirdPersonCameraController cam;
        [SerializeField] private LockOnController lockOn;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private ComboController combo;
        [SerializeField] private SkillComponent skills;
        [SerializeField] private DodgeComponent dodge;
        [SerializeField] private GuardComponent guard;
        [SerializeField] private StaminaComponent stamina;
        [SerializeField] private HealthComponent health;
        [SerializeField] private TowerBuildController build;

        private CharacterController _cc;
        private Vector3 _vel;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (input == null) input = GetComponent<PlayerInput>();
            if (combo == null) combo = GetComponent<ComboController>();
            if (skills == null) skills = GetComponent<SkillComponent>();
            if (dodge == null) dodge = GetComponent<DodgeComponent>();
            if (guard == null) guard = GetComponent<GuardComponent>();
            if (stamina == null) stamina = GetComponent<StaminaComponent>();
            if (health == null) health = GetComponent<HealthComponent>();
            if (attackOrigin == null) attackOrigin = transform;
            if (health != null) health.OnDied += OnDied;
        }

        private void OnDied(GameObject killer) => EventBus.Publish(new PlayerDiedEvent { Player = gameObject });

        private void Update()
        {
            if (health != null && health.IsDead) return;
            if (input == null) return;

            if (cam != null) cam.AddLook(input.Look);
            if (input.LockOnPressed && lockOn != null) lockOn.Toggle();

            // Move
            Vector3 camFwd = cam != null ? cam.ForwardFlat : transform.forward;
            Vector3 camRight = Vector3.Cross(Vector3.up, camFwd);
            Vector3 wish = (camFwd * input.Move.y + camRight * input.Move.x);
            if (wish.sqrMagnitude > 1f) wish.Normalize();
            float speed = walkSpeed * (input.DashHeld ? runMultiplier : 1f);
            if (guard != null && guard.IsGuarding) speed *= 0.5f;

            Vector3 step = wish * speed;
            if (_cc.isGrounded)
            {
                _vel.y = -1f;
                if (input.JumpPressed) _vel.y = jumpSpeed;
            }
            else _vel.y += gravity * Time.deltaTime;

            // Build mode disables attack inputs; routes LMB to build
            if (build != null && build.IsActive)
            {
                if (input.TowerSlot > 0) build.SelectSlot(input.TowerSlot - 1);
                if (input.BasicAttackPressed) build.ConfirmBuild();
                if (input.BuildModeToggled) build.SetActive(false);
            }
            else
            {
                if (input.BuildModeToggled && build != null) build.SetActive(true);
                if (guard != null) guard.SetGuard(input.GuardHeld && _cc.isGrounded);
                if (input.BasicAttackPressed && combo != null) combo.TryAttack(attackOrigin, gameObject);
                if (input.HeavyAttackPressed && skills != null) skills.TryCast(1, attackOrigin, gameObject); // Heavy is C slot fallback
                if (input.Skill1Pressed && skills != null) skills.TryCast(0, attackOrigin, gameObject);
                if (input.Skill2Pressed && skills != null) skills.TryCast(1, attackOrigin, gameObject);
                if (input.Skill3Pressed && skills != null) skills.TryCast(2, attackOrigin, gameObject);
                if (input.UltimatePressed && skills != null) skills.TryCast(3, attackOrigin, gameObject);
                if (input.DodgePressed && dodge != null) StartCoroutine(dodge.Roll(_cc, wish, stamina, 25f));
            }

            // Apply motion
            Vector3 motion = step + new Vector3(0, _vel.y, 0);
            _cc.Move(motion * Time.deltaTime);

            // Face direction
            Vector3 face = lockOn != null && lockOn.Current != null
                ? (lockOn.Current.position - transform.position)
                : wish;
            face.y = 0;
            if (face.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(face);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnLerp * Time.deltaTime);
            }
        }
    }
}
