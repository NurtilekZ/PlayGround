using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace _Src.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Shooter))]
    public class PlayerLocomotion : MonoBehaviour
    {
        public LayerMask aimLayerMask;
        public RigBuilder rigBuilder;
        public MultiAimConstraint weaponAimRig;
        public float maxTargetDistance;
        public Transform targetTransform;
        public Transform aimTransform;
        public Vector3 movementDirection;
        public Vector3 rotationDirection;
    
        [SerializeField] private float moveSpeed;
        [SerializeField] private float maxVelocityChange;
        [SerializeField] private Rigidbody _rigidbody;
        private PlayerInput _playerInput;
        private Camera _mainCamera;
        private Shooter _shooter;
        private Animator _animator;
    
        private static readonly int vertical = Animator.StringToHash("Vertical");
        private static readonly int horizontal = Animator.StringToHash("Horizontal");
        private static readonly int shoot = Animator.StringToHash("Shoot");
        private static readonly int reload = Animator.StringToHash("Reload");

        private void Awake()
        {
            _shooter = GetComponent<Shooter>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;
            _playerInput = new PlayerInput();

        }

        private void Start()
        {
            Application.targetFrameRate = 300;
            _playerInput.Player.Movement.performed += ctx =>
            {
                Vector2 direction = ctx.ReadValue<Vector2>();
                movementDirection = new Vector3(direction.x, 0, direction.y);
            };
            _playerInput.Player.Movement.canceled += ctx => movementDirection = Vector3.zero;
        
            _playerInput.Player.Rotation.performed += ctx =>
            {
                if (ctx.control.device == Gamepad.current)
                {
                    Vector2 direction = ctx.ReadValue<Vector2>() * maxTargetDistance;
                    rotationDirection = new Vector3(direction.x, 1.5f, direction.y) + Vector3.zero;
                }
                else if (ctx.control.device == Mouse.current)
                {
                    Ray ray = _mainCamera.ScreenPointToRay(ctx.ReadValue<Vector2>());
                    if (Physics.Raycast(ray, out var hit, aimLayerMask))
                    {
                        rotationDirection = new Vector3(hit.point.x, 1.5f, hit.point.z);
                    }
                }

            };
            _playerInput.Player.Rotation.canceled += ctx => rotationDirection = transform.forward;
        
            _playerInput.Player.Shoot.performed += ctx => StartCoroutine(Shoot());
        }

        private IEnumerator Shoot()
        {
            while (_playerInput.Player.Shoot.phase == InputActionPhase.Performed)
            {
                if (_shooter.ammo.ammoCount > 0)
                {
                    _shooter.Shoot(aimTransform);
                    _animator.SetTrigger(shoot);
                    _shooter.Muzzle(true);
                    yield return new WaitForSeconds(_shooter.delayTime);
                    _shooter.Muzzle(false);
                }
                else if (!_shooter.isReloading)
                {
                    SetWeights(false);
                    _shooter.isReloading = true;
                    _animator.SetTrigger(reload);
                }
                yield return null;
            }
        }

        // Animation Event
        public void Reload()
        {
            _shooter.Reload();
            SetWeights(true);
        }

        private void SetWeights(bool value)
        {
            rigBuilder.layers[2].active = value;
            rigBuilder.layers[3].active = value;
        }

        private void Update()
        {
            MoveTarget();
            Debug.DrawRay(aimTransform.position, aimTransform.forward * 50f, Color.white);
            float animHor = Vector3.Dot(movementDirection.normalized, transform.right);
            float animVer = Vector3.Dot(movementDirection.normalized, transform.forward);
            
            _animator.SetFloat(vertical, animVer, 0.1f, Time.deltaTime);
            _animator.SetFloat(horizontal, animHor, 0.1f, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = Vector3.Lerp(Vector3.zero, movementDirection, 1);
            targetVelocity *= moveSpeed;
 
            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = _rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        private void MoveTarget()
        {
            targetTransform.position = rotationDirection;
            Vector3.ClampMagnitude(targetTransform.position, maxTargetDistance);

            Vector3 targetDirection = targetTransform.position - transform.position;

            targetDirection.y = 0;

            if (Vector3.Distance(transform.position, targetTransform.position) > 1.0f)
            {
                transform.forward = targetDirection.normalized;
            }
        }

    
        private void OnEnable()
        {
            _playerInput.Player.Enable();
        }

        private void OnDisable()
        {
            _playerInput.Player.Disable();
        }
    }
}