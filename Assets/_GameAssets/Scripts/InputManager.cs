using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Controls _controls;

    public event Action OnReload;
    public event Action OnUse;
    public event Action<int> OnWeaponChange;
    public event Action<bool> OnFire; //send FireIsPressed
    public event Action OnFireStarted;
    public event Action OnFireEnded;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }

    public bool FireIsPressed { get; private set; }

    private void Awake()
    {
        if (InputManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        InputManager.Instance = this;
        _controls = new Controls();
        _controls.PlayerMap.Fire.started += FireStarted;
        _controls.PlayerMap.Fire.canceled += FireEnded;
        _controls.PlayerMap.Reload.performed += ReloadPerformed;
        _controls.PlayerMap.WeaponChange.performed += WeaponChangePerformed;
        _controls.PlayerMap.Use.performed += UsePerformed;
    }

    private void UsePerformed(InputAction.CallbackContext context)
    {
        OnUse?.Invoke();
    }

    private void WeaponChangePerformed(InputAction.CallbackContext context)
    {
        OnWeaponChange?.Invoke((int)context.ReadValue<float>());
    }

    private void ReloadPerformed(InputAction.CallbackContext context)
    {
        OnReload?.Invoke();
    }

    private void FireEnded(InputAction.CallbackContext context)
    {
        FireIsPressed = false;
        OnFireEnded?.Invoke();
    }

    private void FireStarted(InputAction.CallbackContext context)
    {
        OnFireStarted?.Invoke();
        OnFire?.Invoke(false);
        FireIsPressed = true;
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Update()
    {
        JumpPressedThisFrame = _controls.PlayerMap.Jump.triggered;
        MoveInput = _controls.PlayerMap.Move.ReadValue<Vector2>();
        LookInput = _controls.PlayerMap.Look.ReadValue<Vector2>();
        if (FireIsPressed)
        {
            OnFire?.Invoke(FireIsPressed);
        }
    }


}
