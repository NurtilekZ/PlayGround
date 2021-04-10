using System.Collections;
using _Src.Scripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Shooter))]
public class PlayerLocomotion : MonoBehaviour
{
    public LayerMask aimLayerMask;
    public Rig handsRig;
    public MultiAimConstraint weaponAimRig;
    public float maxTargetDistance;
    public Transform targetTransform;
    public Transform aimTransform;
    public Vector3 movementDirection;
    public Vector3 rotationDirection;
    
    private PlayerInput _playerInput;
    private Animator _animator;
    private Camera _mainCamera;
    private Shooter _shooter;
    
    private static readonly int vertical = Animator.StringToHash("Vertical");
    private static readonly int horizontal = Animator.StringToHash("Horizontal");
    private static readonly int shoot = Animator.StringToHash("Shoot");
    private static readonly int reload = Animator.StringToHash("Reload");

    private void Awake()
    {
        Application.targetFrameRate = 300;
        _shooter = GetComponent<Shooter>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;
        
        _playerInput = new PlayerInput();
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
                rotationDirection = new Vector3(direction.x, 1.5f, direction.y) + transform.position;
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
        _playerInput.Player.Rotation.canceled += ctx => rotationDirection = Vector3.zero;
        
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
                yield return new WaitForSeconds(_shooter.delayTime);
            }
            else if (!_shooter.isReloading)
            {
                SetWeights(0);
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
        SetWeights(1);
    }

    private void SetWeights(int value)
    {
        weaponAimRig.weight = value;
        handsRig.weight = value;
    }

    private void Update()
    {
        MoveTarget();
        AnimateMovement();
        Debug.DrawRay(aimTransform.position, aimTransform.forward * 50f, Color.white);
    }

    // private void Move()
    // { 
    //     if (movement.magnitude > 0)
    //     {
    //         movement.Normalize();
    //         movement = movement.normalized * (moveSpeed * Time.deltaTime);
    //         transform.Translate(movement, Space.World);
    //     }
    //
    //     AnimateMovement();
    // }

    private void AnimateMovement()
    {
        float animHor = Vector3.Dot(movementDirection.normalized, transform.right);
        float animVer = Vector3.Dot(movementDirection.normalized, transform.forward);

        _animator.SetFloat(vertical, animVer, 0.1f, Time.deltaTime);
        _animator.SetFloat(horizontal, animHor, 0.1f, Time.deltaTime);
        
    }

    private void MoveTarget()
    {
        if (rotationDirection == Vector3.zero)
        {
            transform.forward = movementDirection.normalized;
            return;
        }
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