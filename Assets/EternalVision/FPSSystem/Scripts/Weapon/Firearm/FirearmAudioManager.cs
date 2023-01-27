using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponSoundsState
{
    fire,
    fireEmpty,
    reload,
    reloadEmpty,
}

public class FirearmAudioManager : FirearmComponent
{
    [SerializeField] private AudioClip _fireClips;
    [SerializeField] private AudioClip _fireEmptyClips;
    [SerializeField] private AudioClip _reloadClip;
    [SerializeField] private AudioClip _reloadClipEmpty;

    [SerializeField] private AudioSettings _fireSettings;
    [SerializeField] private AudioSettings _fireEmptySettings;
    [SerializeField] private AudioSettings _reloadSettings;


    private ObjectPool _objectPool;

    private void Awake()
    {
        _objectPool = GameManager.instance.objPool;

        FirearmShootCompoment.OnFire += OnWeaponFire;
        FirearmShootCompoment.OnFireEmpty += OnWeaponFireEmpty;
        FirearmAmmoComponent.OnReload += OnReload;
    }

    private void OnDestroy()
    {
        FirearmShootCompoment.OnFire -= OnWeaponFire;
        FirearmShootCompoment.OnFireEmpty -= OnWeaponFireEmpty;
        FirearmAmmoComponent.OnReload -= OnReload;
    }

    public void PlaySound(WeaponSoundsState state)
    {
        PooledAudioObject audio = _objectPool.RetrievePoolAudio();
        if (audio == null) return;

        audio.transform.position = transform.position;

        switch (state)
        {
            case WeaponSoundsState.fire:
                audio.SetAudioSettings(_fireSettings, _fireClips);
                break;
            case WeaponSoundsState.fireEmpty:
                audio.SetAudioSettings(_fireEmptySettings, _fireEmptyClips);
                break;
            case WeaponSoundsState.reload:
                audio.transform.parent = this.transform;
                audio.SetAudioSettings(_reloadSettings, _reloadClip);
                break;
            case WeaponSoundsState.reloadEmpty:
                audio.transform.parent = this.transform;
                audio.SetAudioSettings(_reloadSettings, _reloadClipEmpty);
                break;
            default:
                break;
        }

        audio.Play();
    }


    private void OnWeaponFire(WeaponBehaviour weapon, NetworkOwnership networkOwnership)
    {
        if (_currentWeapon == weapon)
            PlaySound(WeaponSoundsState.fire);
    }

    private void OnWeaponFireEmpty(WeaponBehaviour weapon)
    {
        if (_currentWeapon == weapon)
            PlaySound(WeaponSoundsState.fireEmpty);
    } 
    
    private void OnReload(WeaponBehaviour weapon, WeaponSoundsState state)
    {
        if (_currentWeapon == weapon)
            PlaySound(state);
    }

}
