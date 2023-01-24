using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponComponent : NetworkBehaviour
{
    protected MeleeWeapon _currentWeapon;

    public void SetWeaponOwner(MeleeWeapon component)
    {
        _currentWeapon = component;
    }



}
