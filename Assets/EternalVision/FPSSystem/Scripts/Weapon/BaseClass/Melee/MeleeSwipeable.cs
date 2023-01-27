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

    public override void OnUnEquip()
    {
        _currentWeapon.OnUnEquip();
    }
}
