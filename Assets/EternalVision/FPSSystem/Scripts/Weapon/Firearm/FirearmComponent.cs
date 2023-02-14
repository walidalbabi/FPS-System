using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmComponent : NetworkBehaviour
{

    protected WeaponBehaviour _currentWeapon;

    public virtual void SetWeaponOwner(WeaponBehaviour component)
    {
        _currentWeapon = component;
    }
}
