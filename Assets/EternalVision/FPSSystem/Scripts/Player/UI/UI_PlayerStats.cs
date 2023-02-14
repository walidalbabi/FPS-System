using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{

    [Header("PlayerStats")]
    [SerializeField] private Image _healthFill; 
    [SerializeField] private Image _staminaFill;
    [SerializeField] private Text _ammoText;
    [Header("PlayerDamageUI")]
    [SerializeField] private ScreenDamage _screenDamage;
    [Header("UI Bullets")]
    [SerializeField] private Transform _uiAmmoVisualsParent;
    [SerializeField] private UI_Bullet _uiBulletPrefab;
    [Header("HitMarker")]
    [SerializeField] private CanvasGroup _hitMarkerCanvas;

    [Space]
    [Header("Settings")]
    [SerializeField] private float _hitMarkerFadeSpeed = 5f;

    private PlayerHealth _playerHealth;
    private PlayerMovements _playerMovements;
    private PlayerInventoryHandler _playerInventoryHandler;

    private List<UI_Bullet> _uiBulletList = new List<UI_Bullet>();
    private List<UI_Bullet> _uiAciveBulletList = new List<UI_Bullet>();

    private float _currentHealth;
    private float _targetHealth;


    private float _currentHitmarkerFade;
    private float _targetHitmarkerFade;

    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += SetNeededComponents;


        _targetHitmarkerFade = 0f;
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= SetNeededComponents;

        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= OnHealthChange;
            _playerHealth.OnRespawned -= OnRespawned;
        }

        if (_playerInventoryHandler != null)
        {
            _playerInventoryHandler.OnSelectNewItem -= OnNewItemSelected;
        }


        FirearmAmmoComponent.OnAmmoChange -= OnAmmoChange;
        FirearmShootCompoment.OnHitATarget -= OnHitTarget;
    }


    private void LateUpdate()
    {
        //Calculate Health
        if(_currentHealth != _targetHealth)
        {
            _currentHealth = Mathf.Lerp(_currentHealth, _targetHealth, Time.deltaTime *3f);
            _healthFill.fillAmount = _currentHealth;
        }

        //Set Stamina
        if(_staminaFill != null && _playerMovements != null)
        {
       //     _staminaFill.fillAmount = _playerMovements.currentStamina / 100f;
            _staminaFill.fillAmount = Mathf.Lerp(_staminaFill.fillAmount, _playerMovements.currentStamina / 100f, Time.deltaTime * 5f);
        }

        //Set hit Marker
        if (_currentHitmarkerFade != _targetHitmarkerFade)
        {
            _currentHitmarkerFade = Mathf.Lerp(_currentHitmarkerFade, _targetHitmarkerFade, Time.deltaTime * _hitMarkerFadeSpeed);
            _hitMarkerCanvas.alpha = Mathf.Max(0f, _currentHitmarkerFade);
        }
    }

    public void SetNeededComponents(GameObject playerObj)
    {
        _playerHealth = playerObj.GetComponent<PlayerHealth>();
        _playerMovements = playerObj.GetComponent<PlayerMovements>();
        _playerInventoryHandler = playerObj.GetComponent<PlayerInventoryHandler>();

        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged += OnHealthChange;
            _playerHealth.OnRespawned += OnRespawned;
        }

        if(_playerInventoryHandler != null)
        {
            _playerInventoryHandler.OnSelectNewItem += OnNewItemSelected;
        }

        FirearmAmmoComponent.OnAmmoChange += OnAmmoChange;
        FirearmShootCompoment.OnHitATarget += OnHitTarget;

        InitAmmoUIVisuals();
    }

    private void InitAmmoUIVisuals()
    {
        for (int i = 0; i < 150; i++)
        {
            var uiBullet = Instantiate(_uiBulletPrefab, _uiAmmoVisualsParent);
            uiBullet.gameObject.hideFlags = HideFlags.HideInHierarchy;
            uiBullet.gameObject.SetActive(false);
            _uiBulletList.Add(uiBullet);
        }
    }

    private void OnHealthChange(int oldHealth, int newHealth, int maxHealth)
    {
        _targetHealth = newHealth;
        _targetHealth = _targetHealth / maxHealth;
        _currentHealth = oldHealth;
        _currentHealth = _currentHealth / maxHealth;


        _screenDamage.CurrentHealth = newHealth;
    }

    private void OnRespawned()
    {
        _targetHealth = _playerHealth.MaximumHealth / _playerHealth.MaximumHealth;
        _screenDamage.CurrentHealth = _playerHealth.MaximumHealth;
    }


    private void OnAmmoChange(WeaponBehaviour weapon, int oldAmmo, int newAmmo, int maxAmmo)
    {
        if (weapon != _playerInventoryHandler.currentSelectedPlayerItem) return;

        MatchUIBulletsToCurrentAmmo(weapon);
        _ammoText.text = "/ " + weapon.weaponAmmoComponent.totalAmmo;
    }

    private void OnHitTarget(WeaponBehaviour weapon)
    {
        if (weapon != _playerInventoryHandler.currentSelectedPlayerItem) return;

        _currentHitmarkerFade = 1f;
        _targetHitmarkerFade = 0f;
    }

    private void OnNewItemSelected(int index, SwipeableItemClass item)
    {
        WeaponBehaviour weapon = item.GetComponent<WeaponBehaviour>();

        //if is weapon
        if (weapon != null)
        {
            _uiAmmoVisualsParent.gameObject.SetActive(true);
            ResetAllUIBullets();
            SetUIBulletVisualsToCurrentNewWeapon(weapon);

            MatchUIBulletsToCurrentAmmo(weapon);
            _ammoText.text =  "/ " + weapon.weaponAmmoComponent.totalAmmo;
        }
        else
        {
            _uiAmmoVisualsParent.gameObject.SetActive(false);
            _ammoText.text = "";
        }
    }


    private void ResetAllUIBullets()
    {
        foreach (var item in _uiBulletList)
        {
            item.gameObject.SetActive(false);
            item.transform.parent = null;
            _uiAciveBulletList.Remove(item);
        }
    }

    private void SetUIBulletVisualsToCurrentNewWeapon(WeaponBehaviour weapon)
    {
        for (int i = 0; i < weapon.weaponAmmoComponent.maxAmmoMag; i++)
        {
            _uiAciveBulletList.Add(_uiBulletList[i]);
            _uiAciveBulletList[i].transform.parent = _uiAmmoVisualsParent;
            _uiAciveBulletList[i].gameObject.SetActive(true);
            _uiAciveBulletList[i].SetBullet(weapon.bulletUIImage);
        }
    }

    private void MatchUIBulletsToCurrentAmmo(WeaponBehaviour weapon)
    {
        for (int i = 0; i < weapon.weaponAmmoComponent.maxAmmoMag; i++)
        {
            if(i <= weapon.weaponAmmoComponent.currentAmmoCount - 1)
            {
                _uiAciveBulletList[i].FullBullet();
            }
            else
            {
                _uiAciveBulletList[i].EmptyBullet();
            }
        }
    }


}


