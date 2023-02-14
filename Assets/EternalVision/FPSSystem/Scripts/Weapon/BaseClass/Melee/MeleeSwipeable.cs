using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSwipeable : SwipeableItemClass
{
    private MeleeWeapon _currentWeapon;

    private void Awake()
    {
        _currentWeapon = GetComponent<MeleeWeapon>();

    }

    public override void OnEquip()
    {
        _currentWeapon.OnEquip();

        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {

            child.gameObject.layer = transform.parent.gameObject.layer;
        }
    }

    /// <summary>
    /// is changing item true, if we need to enable the next selected item after disabling current one
    /// </summary>
    /// <param name="isChangingItem"></param>
    public override void OnUnEquip(bool isChangingItem)
    {
        _currentWeapon.OnUnEquip(isChangingItem);
    }
}
