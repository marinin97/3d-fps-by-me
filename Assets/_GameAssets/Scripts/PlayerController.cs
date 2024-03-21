using Cinemachine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    public event Action<Weapon> OnWeaponChanged;
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    [field: SerializeField]
    public float MaxHealth { get; private set; }
    public float Health { get; private set; }

    [SerializeField]
    private CharacterController _characterController;

    [SerializeField]
    [Min(0.1f)]
    private float _moveSpeed;
    [SerializeField]
    [Min(0.1f)]
    private float _lookSpeed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _gravityAcceleration = -9.81f;

    [Header("Cameras")]
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    private float _maxCameraPitch;
    [SerializeField]
    private float _minCameraPitch;

    [Header("E TEXT")]
    [SerializeField]
    private TMP_Text _useText;
    [SerializeField]
    private float _maxUseDistance = 5f;
    [SerializeField]
    private LayerMask _useLayers;

    [field: SerializeField]
    public WeaponChanger WeaponChanger { get; private set; }

    private InputManager _inputManager;
    private Transform _transform;
    private Vector3 _playerVelocity;
    private float _cameraTargetPitch;
    private bool _isDead;


    private void Awake()
    {
        Health = MaxHealth;
        _inputManager = InputManager.Instance;
        _inputManager.OnFire += Fire;
        _inputManager.OnReload += Reload;
        _inputManager.OnWeaponChange += ChangeWeapon;
        _inputManager.OnUse += OnUseClicked;

        _transform = _characterController.transform;
        _cameraTransform = _cameraTransform != null ? _cameraTransform : Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        WeaponChanger.OnWeaponChanged += HandleWeaponChange;
        WeaponChanger.ChangeWeapon(0);
    }
    private void Start()
    {
        OnHealthChanged?.Invoke(Health);
    }


    private void HandleWeaponChange(Weapon weapon)
    {
        OnWeaponChanged?.Invoke(WeaponChanger.CurrentWeapon);
    }

    private void ChangeWeapon(int weaponIndex)
    {
        WeaponChanger.ChangeWeapon(weaponIndex);
    }

    private void Reload()
    {
        WeaponChanger.CurrentWeapon.Reload();
    }

    private void Fire(bool fireIsPressed)
    {
        WeaponChanger.CurrentWeapon.Shoot(fireIsPressed, _cameraTransform);
    }

    private void OnValidate()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }
        ApplyGravity();
        ApplyMove();
        Rotate();
        Jump();
        _characterController.Move(_playerVelocity * Time.deltaTime);

        OnUseSetText();
    }

    private void Rotate()
    {
        if (_inputManager.LookInput.sqrMagnitude < 0.01f)
        {
            return;
        }
        _cameraTargetPitch += _inputManager.LookInput.y * _lookSpeed * Time.deltaTime;
        _cameraTargetPitch = Mathf.Clamp(_cameraTargetPitch, _minCameraPitch, _maxCameraPitch);
        _cameraTransform.localRotation = Quaternion.Euler(_cameraTargetPitch, 0, 0);
        _transform.Rotate(Vector3.up * _inputManager.LookInput.x * _lookSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _playerVelocity.y += _gravityAcceleration * Time.deltaTime;
        if (_characterController.isGrounded)
        {
            _playerVelocity.y = -0.1f;
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded && InputManager.Instance.JumpPressedThisFrame)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -2 * _gravityAcceleration);
        }
    }

    private void ApplyMove()
    {
        _playerVelocity.x = _playerVelocity.z = 0;

        var moveInput = _inputManager.MoveInput;
        var moveVelocity = (_transform.forward * moveInput.y + _transform.right * moveInput.x).normalized * _moveSpeed;
        _playerVelocity += moveVelocity;
    }

    private void OnDestroy()
    {
        if (_inputManager != null)
        {
            _inputManager.OnFire -= Fire;
            _inputManager.OnReload -= Reload;
        }

        WeaponChanger.OnWeaponChanged -= HandleWeaponChange;
    }

    public void TakeDamage(float damageValue)
    {
        Health -= damageValue;
        if (Health <= 0)
        {
            Health = 0;
            _isDead = true;
            OnDeath?.Invoke();
            WeaponChanger.DisableAllWeapons();
        }

        OnHealthChanged?.Invoke(Health);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.point.y >= _transform.position.y + _characterController.height / 2)
        {
            _playerVelocity.y = -1;
        }
    }

    public void OnUseClicked()
    {
        Debug.Log("E button pressed");
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _maxUseDistance, _useLayers))
        {
            if (hit.collider.TryGetComponent<Door>(out Door door))
            {
                if (door.IsOpen)
                {
                    door.Close();
                }
                else
                {
                    door.Open(_transform.position);
                }
            }
        }
    }

    private void OnUseSetText()
    {
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _maxUseDistance, _useLayers)
           && hit.collider.TryGetComponent<Door>(out Door door))
        {
            if (door.IsOpen)
            {
                _useText.SetText("Close \"E\"");
            }
            else
            {
                _useText.SetText("Open \"E\"");
            }

            _useText.gameObject.SetActive(true);
            _useText.transform.position = hit.point - (hit.point - _cameraTransform.position).normalized * 0.01f;
            _useText.transform.rotation = Quaternion.LookRotation((hit.point - _cameraTransform.position).normalized);
        }
        else
        {
            _useText.gameObject.SetActive(false);
        }
    }

}
