using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponShootType
{
    Auto,
    SemiAuto
}

public abstract class WeaponBehaviour : PlayerItem
{

    //Serilized
    [SerializeField] private Transform _cameraView;
    //Protected Members
    protected bool _isAiming;

    protected FirearmShootCompoment _shootComponent;
    protected FirearmAimComponent _weaponAimComponent;
    protected FirearmAmmoComponent _weaponAmmoComponent;
    protected WeaponRunComponent _firearmRunComponent;
    protected FirearmAudioManager _weaponAudioManager;

    protected PlayerMovements _playerMovements;

    protected NetworkOwnership _ownership;


    //Private Members


    //Public Members
    public WeaponShootType weaponShootType;
    public Sprite bulletUIImage;

    //Properties
    public bool isAiming { get { return _isAiming; } }
    public PlayerInventoryHandler playerInventoryHandler { get { return _playerInventoryHandler; } }
    public FirearmShootCompoment shootComponent { get { return _shootComponent; } }
    public FirearmAimComponent weaponAimComponent { get { return _weaponAimComponent; } }
    public FirearmAmmoComponent weaponAmmoComponent { get { return _weaponAmmoComponent; } }
    public FirearmAudioManager weaponAudioManager { get { return _weaponAudioManager; } }
    public NetworkOwnership ownership { get { return _ownership; } }


    public virtual void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        _shootComponent = GetComponentInChildren<FirearmShootCompoment>();
        _weaponAimComponent = GetComponentInChildren<FirearmAimComponent>();
        _weaponAmmoComponent = GetComponent<FirearmAmmoComponent>();
        _firearmRunComponent = GetComponentInChildren<WeaponRunComponent>();
        _weaponAudioManager = GetComponent<FirearmAudioManager>();
        _itemSwayMotion = GetComponent<ItemSwayMotion>();
        _itemBobbingMotion = GetComponent<ItemBobbingMotion>();

        if (_shootComponent != null)
            FirearmShootCompoment.OnFire += OnWeaponFire;

        if (_weaponAimComponent != null)
            FirearmAimComponent.OnAimStateChange += OnAimStateChange;

        foreach (var component in GetComponentsInChildren<FirearmComponent>())
        {
            component.SetWeaponOwner(this);
        }
    }

    private void OnDestroy()
    {
        if (_shootComponent != null)
            FirearmShootCompoment.OnFire -= OnWeaponFire;

        if (_weaponAimComponent != null)
            FirearmAimComponent.OnAimStateChange -= OnAimStateChange;
    }

    public abstract void OnEquip();
    public abstract void OnUnEquip();

    public virtual void Reload() { }
    public virtual void Shoot() { }
    public virtual void Aim(bool state) { }


    public Transform GetCameraView() => _cameraView;

  

    public void OnWeaponFire(WeaponBehaviour weapon, NetworkOwnership ownership)
    {

        if (weapon != this) return;

        //Trigger Animations of FP Components
        foreach (var anim in animators)
        {
            anim.SetTrigger("Fire");
        }
    }

    private void OnAimStateChange(WeaponBehaviour weapon, bool state)
    {
        if (weapon != this) return;

        //Trigger Animations of FP Components
        foreach (var anim in animators)
        {
            anim.SetBool("IsAiming", state);
        }
    }
}
