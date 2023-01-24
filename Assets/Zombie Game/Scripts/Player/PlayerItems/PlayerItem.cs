using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemType
{
    ModulareFirearm,
    MeleeWeapon,
    Throwable,
    Consumable
}

[RequireComponent(typeof(ItemBobbingMotion), typeof(ItemSwayMotion))]
public abstract class PlayerItem : NetworkBehaviour
{

    public ItemType currentItemType;
    public Vector3 weaponOffset;

    [HideInInspector] public Animator[] animators;

    protected PlayerInventoryHandler _playerInventoryHandler;
    protected NetworkPlayerFullBodyAnimationHandler _fullBodyAnimatorHandler;
    protected ItemSwayMotion _itemSwayMotion;
    protected ItemBobbingMotion _itemBobbingMotion;

    public virtual void OnRightClick() { }
    public virtual void OnLeftClick() { }
    public virtual void OnLeftClick(bool state) { }

    public virtual void OnRightClick(bool state) { }
    public virtual void OnLeftClick(Vector3 position, Vector3 direction) { }


    public virtual void OnPressReloadBtn() { }

    public void Set_FullBodyAnimatorHandler(NetworkPlayerFullBodyAnimationHandler component)
    {
        _fullBodyAnimatorHandler = component;
    }
    public virtual void Set_PlayerInventoryHandler(PlayerInventoryHandler component)
    {
        _playerInventoryHandler = component;
    }

    public virtual void SetWeaponComponentsNeeds(NetworkOwnership ownership, PlayerMovements _playerMovements)
    {
    }

    public LocalPlayerData GetLocalPlayerDataOfThisItem()
    {
        return _playerInventoryHandler.gameObject.GetComponent<LocalPlayerData>();
    }
}
