using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MeleeWeaponSoundState
{
    Swing,
    HoldSwing
}

public class MeleeWeaponAudioManager : MeleeWeaponComponent
{
    [SerializeField] private AudioClip[] _swingClips;
    [SerializeField] private AudioClip _holdSwingClips;


    [SerializeField] private AudioSettings _swingSettings;
    [SerializeField] private AudioSettings _holdSwingSettings;


    private ObjectPool _objectPool;

    private void Awake()
    {
        _objectPool = GameManager.instance.objPool;

        MeleeWeapon.OnSwing += OnSwing;
        MeleeWeapon.OnHoldSwing += OnHoldSwing;
    }

    private void OnDestroy()
    {
        MeleeWeapon.OnSwing -= OnSwing;
        MeleeWeapon.OnHoldSwing -= OnHoldSwing;
    }

    public void PlaySound(MeleeWeaponSoundState state)
    {
        PooledAudioObject audio = _objectPool.RetrievePoolAudio();
        if (audio == null) return;

        audio.transform.position = transform.position;

        switch (state)
        {
            case MeleeWeaponSoundState.Swing:
                audio.SetAudioSettings(_swingSettings, _swingClips[Random.Range(0, _swingClips.Length)]);
                break;
            case MeleeWeaponSoundState.HoldSwing:
                audio.SetAudioSettings(_holdSwingSettings, _holdSwingClips);
                break;
            default:
                break;
        }

        audio.Play();
    }


    private void OnSwing(int swingIndex, MeleeWeapon weapon)
    {
        if (_currentWeapon == weapon)
            PlaySound(MeleeWeaponSoundState.Swing);
    }

    private void OnHoldSwing(MeleeWeapon weapon)
    {
        if (_currentWeapon == weapon)
            PlaySound(MeleeWeaponSoundState.HoldSwing);
    }

}
