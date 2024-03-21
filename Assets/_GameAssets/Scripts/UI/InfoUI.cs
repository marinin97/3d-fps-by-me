using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private Slider _healthBar;
    [SerializeField]
    private TMP_Text _healthBarText;

    [Header("Ammo")]
    [SerializeField]
    private GameObject _ammoInfo;
    [SerializeField]
    private TMP_Text _magazineAmmo;
    [SerializeField]
    private TMP_Text _totalAmmo;

    [Header("Wave")]
    [SerializeField]
    private GameObject _waveInfo;
    [SerializeField]
    private TMP_Text _waveCountText;

    [Header("Kill count")]
    [SerializeField]
    private TMP_Text _killCountText;

    [Header("EndGame Screen")]
    [SerializeField]
    private GameObject _endScreen;
    [SerializeField]
    private Button _retryButton;
    [SerializeField]
    private GameObject _crosshair;

    [Header("Components")]
    [SerializeField]
    private PlayerController _playerController;
    [SerializeField]
    private EnemySpawner _enemySpawner;

    private Weapon _currentWeapon;

    private void Awake()
    {
        _playerController.OnWeaponChanged += HandleWeaponChange;
        _playerController.OnDeath += OnPlayerDeath;
        _healthBar.maxValue = _playerController.MaxHealth;
        _playerController.OnHealthChanged += SetHealthBarValue;
        _enemySpawner.OnWaveStarted += SetWaveInfo;
        _enemySpawner.OnEnemyKilled += SetKillCount;
    }

    private void OnPlayerDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _ammoInfo.SetActive(false);
        _endScreen.SetActive(true);
        _playerController.OnHealthChanged -= SetHealthBarValue;
        _healthBar.gameObject.SetActive(false);
        _healthBarText.gameObject.SetActive(false);
        _crosshair.SetActive(false);
        _retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    private void SetKillCount(int killCount)
    {
        _killCountText.text = killCount.ToString();
    }

    private void SetWaveInfo(int waveCount)
    {
        if (!_waveInfo.activeInHierarchy)
        {
            _waveInfo.SetActive(true);
        }

        _waveCountText.text = waveCount.ToString();
    }

    private void SetHealthBarValue(float value)
    {
        _healthBar.value = value;
        _healthBarText.text = ((int)Math.Ceiling(value)).ToString();
    }

    private void HandleWeaponChange(Weapon weapon)
    {
        Unsubscribe(_currentWeapon);
        _currentWeapon = weapon;
        Subscribe(_currentWeapon);
        SetCounter(_currentWeapon);
    }

    private void SetCounter(Weapon currentWeapon)
    {
        _ammoInfo.SetActive(currentWeapon is not Knife);
        _totalAmmo.text = currentWeapon.TotalAmmo.ToString();
        _magazineAmmo.text = currentWeapon.CurrentMagazineAmmo.ToString();
    }

    private void OnDestroy()
    {
        Unsubscribe(_currentWeapon);
        _playerController.OnWeaponChanged -= HandleWeaponChange;
    }

    private void Subscribe(Weapon weapon)
    {
        if (weapon != null)
        {
            weapon.OnReload += SetCounter;
            weapon.OnShoot += SetCounter;
        }
    }

    private void Unsubscribe(Weapon weapon)
    {
        if (weapon != null)
        {
            weapon.OnReload -= SetCounter;
            weapon.OnShoot -= SetCounter;
        }
    }
}
