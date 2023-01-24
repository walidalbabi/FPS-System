using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularFirearm : WeaponBehaviour
{
    [SerializeField] private WeaponThirdPersonModule _weaponThirdPersonModule;
    /// <summary>
    /// The instantiated weapon for the fullbody used if we want to instantiate a bullet there for visuals , also for playing other visuals
    /// </summary>
    private WeaponThirdPersonModule _weaponModule;
    public WeaponThirdPersonModule weaponModule { get { return _weaponModule; } }

    public override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        //set weapon Sway Settings
        if (_itemSwayMotion != null) _itemSwayMotion.SetWeaponSway();
        if (_itemBobbingMotion != null) _itemBobbingMotion.SetBobSettings();
    }

    public override void Set_PlayerInventoryHandler(PlayerInventoryHandler component)
    {
        base.Set_PlayerInventoryHandler(component);
        _weaponModule = Instantiate(_weaponThirdPersonModule, _playerInventoryHandler.weaponThirdPersonHolder);
        _weaponModule.transform.localPosition = Vector3.zero;
    }

    public override void OnEquip()
    {
        if (_weaponModule != null)
        {
            _weaponModule.gameObject.SetActive(true);
            _weaponModule.transform.localPosition = Vector3.zero;
        }

    }

    public override void SetWeaponComponentsNeeds(NetworkOwnership ownership, PlayerMovements _playerMovements)
    {
        if (ownership.isOwner)
            _weaponAimComponent.SetPlayerCamera();

        this._playerMovements = _playerMovements;

        if (this._playerMovements == null) return;

        _firearmRunComponent.SetPlayermovementsComponent(_playerMovements);
    }

    public override void OnUnEquip()
    {
        //Trigger The Unequip Animation
        foreach (var anim in animators)
        {
            anim.SetTrigger("Unequip");
        }
        //Unquipe Weapon
        StartCoroutine(UnequipWeapon(animators[0].GetFloat("EquipSpeed")));
    }



    public override void OnLeftClick()
    {
        Shoot();
    }

    public override void OnRightClick(bool state)
    {
        Aim(state);
    }

    public override void OnPressReloadBtn()
    {
        Reload();
        ServerReload();
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        //Trigget Shooting component
        _shootComponent.Shoot();


        if (_weaponAmmoComponent.isEmpty) return;
        _fullBodyAnimatorHandler.SetFire();
    }

    public override void Reload()
    {
        if (_weaponAmmoComponent.CanReload())
        {
            _weaponAimComponent.ForceUnAim();

            if (_weaponAmmoComponent.isEmpty)
            {
                _weaponAmmoComponent.Event_CallOnReload(this, WeaponSoundsState.reloadEmpty);
                foreach (var anim in animators)
                {
                    anim.SetTrigger("Empty Reload");
                }
            }
            else
            {
                _weaponAmmoComponent.Event_CallOnReload(this, WeaponSoundsState.reload);
                foreach (var anim in animators)
                {
                    anim.SetTrigger("Reload");
                }
            }


            _fullBodyAnimatorHandler.SetReload();

            _weaponAmmoComponent.Event_CallOnReloadStateChange(this, true);
            _weaponAmmoComponent.isReloading = true;
        }
  
    }


    [ServerRpc]
    private void ServerReload()
    {
        if (!base.IsServer) return;
        Reload();
        ObserversReload();
    }

    [ObserversRpc]
    private void ObserversReload()
    {
        if (base.IsOwner || base.IsServer) return;
        Reload();
    }

    public override void Aim(bool state)
    {
        _isAiming = state;
        if (base.IsClient)
            _weaponAimComponent.ToggleISAiming(state);
    }


    [ServerRpc]
    private void ServerAim(bool state)
    {
        if (!base.IsServer) return;
        Aim(state);
        ObserversAim(state);
    }

    [ObserversRpc]
    private void ObserversAim(bool state)
    {
        if (base.IsOwner || base.IsServer) return;
        Aim(state);
    }


    private bool CanShoot()
    {
        if (_weaponAmmoComponent.isReloading) return false;
        if (_weaponAmmoComponent.isReloading) return false;


        return true;
    }


    IEnumerator UnequipWeapon(float delay)
    {
        //Waiting unquip animation to finish
        yield return new WaitForSeconds(delay);
        //Equip New Weapon
        _playerInventoryHandler.EnableNewItem();
        //Disable Current Weapon
        _weaponModule.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


}
