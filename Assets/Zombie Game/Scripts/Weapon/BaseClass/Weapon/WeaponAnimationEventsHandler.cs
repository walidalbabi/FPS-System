using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationEventsHandler : MonoBehaviour
{
    [SerializeField] private WeaponBehaviour _weaponBehaviour;

    public void OnReloadEmpty()
    {
        _weaponBehaviour.weaponAmmoComponent.Reload();
    }
}
