using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : PlayerItem
{

    [SerializeField] private WeaponThirdPersonModule _weaponThirdPersonModule;
    [Header("Melee Weapon Settings")]
    [SerializeField] private bool _canUseHoldAttack;
    [SerializeField] private float _minTimeHold = 0.4f;
    [SerializeField] private float _speedRate = 0.4f;
    [SerializeField] private int _weaponDamage = 10;
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private MeleeDamageTrigger _meleeDamageTriggerRight;
    [SerializeField] private MeleeDamageTrigger _meleeDamageTriggerLeft;


    private float _holdTimer;
    private bool _isHoldAttack;
    private float _lastSwingTime;
    private int _currentAttackIndex;


    /// <summary>
    /// The instantiated weapon for the fullbody used if we want to instantiate a bullet there for visuals , also for playing other visuals
    /// </summary>
    private WeaponThirdPersonModule _weaponModule;


    public PlayerInventoryHandler playerInventoryHandler { get { return _playerInventoryHandler; } }
    public WeaponThirdPersonModule weaponModule { get { return _weaponModule; } }


    public static event Action<int, MeleeWeapon> OnSwing;
    public static event Action<MeleeWeapon> OnHoldSwing;



    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var component in GetComponentsInChildren<MeleeWeaponComponent>())
        {
            component.SetWeaponOwner(this);
        }
    }

    private void Start()
    {
        if (_meleeDamageTriggerRight != null) _meleeDamageTriggerRight.SetSettings(_weaponDamage, _collisionLayer, _playerInventoryHandler.gameObject.GetHashCode());
        if (_meleeDamageTriggerLeft != null) _meleeDamageTriggerLeft.SetSettings(_weaponDamage, _collisionLayer, _playerInventoryHandler.gameObject.GetHashCode());
    }


    public  void OnEquip()
    {
        if (_weaponModule != null)
        {
            _weaponModule.gameObject.SetActive(true);
            _weaponModule.transform.localPosition = Vector3.zero;
        }
    }

    public  void OnUnEquip()
    {
        //Trigger The Unequip Animation
        foreach (var anim in animators)
        {
            anim.SetTrigger("Unequip");
        }
        //Unquipe Weapon
        StartCoroutine(UnequipWeapon(animators[0].GetFloat("EquipSpeed")));
    }

    IEnumerator UnequipWeapon(float delay)
    {
        //Waiting unquip animation to finish
        yield return new WaitForSeconds(delay);
        //Equip New Weapon
        _playerInventoryHandler.EnableNewItem();
        //Disable Current Weapon
        if (_weaponModule != null)
            _weaponModule.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Time.time > (_lastSwingTime + _speedRate))
        {
            if (_meleeDamageTriggerRight != null)
            {
                _meleeDamageTriggerRight.DisableTrigger();
            }
            if (_meleeDamageTriggerLeft != null)
            {
                _meleeDamageTriggerLeft.DisableTrigger();
            }
        }

    }
    public override void OnLeftClick(bool state)
    {

        if (Time.time <= (_lastSwingTime + _speedRate)) return;


        if (state)
        {
            if (_canUseHoldAttack)
            {
                Debug.Log("Hold Swing");

                _holdTimer += Time.deltaTime;

                if (_holdTimer > _minTimeHold)
                {
                    HoldAttack();
                }
            }
            else
            {
                OnRelease();
            }
        }
        else
        {
            if (_canUseHoldAttack)
                OnRelease();
        }

    }


    public void OnRelease()
    {
        Debug.Log(" Swing");
        if (_holdTimer <= _minTimeHold)
        {
            PlayNormalAttack();

        }
        else if (_isHoldAttack)
        {
            PlayHoldAttack();
        }

        _holdTimer = 0;
        _lastSwingTime = Time.time;
        _isHoldAttack = false;
    }


    private void HoldAttack()
    {
        _isHoldAttack = true;
        foreach (var anim in animators)
        {
            anim.SetBool("Hold", true);
        }

        _meleeDamageTriggerRight.EnableTrigger();

        OnHoldSwing?.Invoke(this);
    }

    private void PlayNormalAttack()
    {
        foreach (var anim in animators)
        {
            anim.SetTrigger("Swing");
        }
        foreach (var anim in animators)
        {
            anim.SetFloat("Swing Index", (float)_currentAttackIndex);
        }

        OnSwing?.Invoke(_currentAttackIndex, this);

        if (_currentAttackIndex % 2 == 0)
        {
            _meleeDamageTriggerRight.EnableTrigger();
        }
        else
        {
           if(_meleeDamageTriggerLeft != null) _meleeDamageTriggerLeft.EnableTrigger();
            else _meleeDamageTriggerRight.EnableTrigger();
        }

        _currentAttackIndex = _currentAttackIndex < 3 ? _currentAttackIndex + 1 : 0;
    }

    private void PlayHoldAttack()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("Hold", false);
        }
    }

}
