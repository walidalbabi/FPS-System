using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerController : NetworkBehaviour
{
    protected PlayerInventoryHandler _playerInventoryHandler;
    protected NetworkPlayerFullBodyAnimationHandler _fullBodyAnimatorHandler;
    protected LocalPlayerData _localPlayerActionData;
    protected InputHandler _inputHandler;
    protected InputData _networkInputs;

    public static event Action<Camera> OnCameraTargetChanged;
    public static event Action<GameObject> OnPlayerAvailbleInScene;

    public virtual void Awake()
    {
        _playerInventoryHandler = GetComponent<PlayerInventoryHandler>();
        _fullBodyAnimatorHandler = GetComponent<NetworkPlayerFullBodyAnimationHandler>();
        _localPlayerActionData = GetComponent<LocalPlayerData>();
        _inputHandler = GetComponent<InputHandler>();
    }

    public virtual void GetInputs()
    {
        _networkInputs = default;
        _networkInputs = _inputHandler.GetPlayerLocalInputs();
        _inputHandler.NeededResetCashedInputsReload();
    }


    /// <summary>
    /// Check for inputs if We Pressed LeftClick and do an action based 
    /// On the current selected Item
    /// </summary>
    public virtual void CheckForLeftClick()
    {
        if (_networkInputs.LeftClick) // we pressed LeftClick
        {
            if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType == ItemType.ModulareFirearm) // we have a gun
            {
                if (!CanShoot()) return;

                if (_playerInventoryHandler.GetCurrentEquipedWeapon() == null) return; // Make Sure we have a selected gun
                //check Weapon Shooting type
                if (_playerInventoryHandler.GetCurrentEquipedWeapon().weaponShootType == WeaponShootType.SemiAuto) _inputHandler.NeededResetCashedInputsFire();

                _localPlayerActionData.isFiring = true;
                _playerInventoryHandler.currentSelectedPlayerItem.OnLeftClick();
            }

            if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType == ItemType.MeleeWeapon) // we have a melee Weapon
            {
                if (!CanMelee()) return;

                _localPlayerActionData.isMeleeAttack = true;
                _playerInventoryHandler.currentSelectedPlayerItem.OnLeftClick(true);
            }

        }
        else // we released LeftClick
        {

            if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType == ItemType.ModulareFirearm)// we have a gun
            {
                //Reset Fire for Condition
                _localPlayerActionData.isFiring = false;
            }

            if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType == ItemType.MeleeWeapon && _localPlayerActionData.isMeleeAttack) // we have a melee Weapon
            {
                //Reset Melee Attack for Conditions
                _localPlayerActionData.isMeleeAttack = false;
                _playerInventoryHandler.currentSelectedPlayerItem.OnLeftClick(false);// Release the action, used here for Hold Attack's
            }
        }
    }

    /// <summary>
    /// Check for inputs if We Pressed RightClick and do an action based 
    /// On the current selected Item
    /// </summary>
    public virtual void CheckForRightClick() 
    {

        if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType == ItemType.ModulareFirearm) // IF we Have a gun
        {
            if (CanAim()) // If we can Aim
            {
                if (_networkInputs.RightClick)// Aim Down Sight
                {
                    _playerInventoryHandler.currentSelectedPlayerItem.OnRightClick(true);
                    _localPlayerActionData.isAiming = true;

                }

                else // UnAim
                {
                    _localPlayerActionData.isAiming = false;

                    _playerInventoryHandler.currentSelectedPlayerItem.OnRightClick(false);
                }
            }
            else // if we Can't Aim Force UnAim
            {
                _localPlayerActionData.isAiming = false;

                _playerInventoryHandler.currentSelectedPlayerItem.OnRightClick(false);
            }

        }

    }


    /// <summary>
    /// Check for inputs if we want to Reload
    /// </summary>
    public virtual void CheckForReload()
    {
        if (_playerInventoryHandler.currentSelectedPlayerItem.currentItemType != ItemType.ModulareFirearm) return;
        if (!CanReload()) return;

        if (_networkInputs.reload)
        {
            if (_playerInventoryHandler.currentSelectedPlayerItem == null) return;
            _networkInputs.reload = false;
            _playerInventoryHandler.currentSelectedPlayerItem.OnPressReloadBtn();
            Reload();
        }
    }


    /// <summary>
    /// Check for inputs if we want to change item
    /// </summary>
    public virtual void CheckForChangingItem()
    {
        if (!CanSwitchItem()) return;

        if (_networkInputs.itemScroll.y > 0)
        {
            _playerInventoryHandler.ChangeItem(_playerInventoryHandler.GetCurrentItemIndex() + 1);
        }
        else if (_networkInputs.itemScroll.y < 0)
        {
            _playerInventoryHandler.ChangeItem(_playerInventoryHandler.GetCurrentItemIndex() - 1);
        }
    }

    public virtual void Reload()
    {
        _playerInventoryHandler.currentSelectedPlayerItem.OnPressReloadBtn();
    }

    public void Event_OnCameraTargetChanged(GameObject cam)
    {
        OnCameraTargetChanged?.Invoke(cam.GetComponent<Camera>());
    }

    public void Event_CallOnPlayerAvailbleInScene()
    {
        OnPlayerAvailbleInScene?.Invoke(this.gameObject);
    }


    public bool CanShoot()
    {
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_localPlayerActionData.isSwitchingItem) return false;

        return true;
    }

    public bool CanMelee()
    {
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_localPlayerActionData.isSwitchingItem) return false;

        return true;
    }

    public bool CanAim()
    {
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_localPlayerActionData.isSwitchingItem) return false;

        return true;
    }
    public bool CanSwitchItem()
    {
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_localPlayerActionData.isSwitchingItem) return false;

        return true;
    }

    public bool CanReload()
    {
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isFiring) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_localPlayerActionData.isSwitchingItem) return false;

        return true;
    }


}
