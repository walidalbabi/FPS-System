using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledAudioBulletImpact : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSettings _audioSettings;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        SetAudioSettings();
    }

    private void OnEnable()
    {
        OnPlay();
    }

    public void SetAudioSettings()
    {
        _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Length)];
        _audioSource.volume = _audioSettings.volume;
        _audioSource.pitch = _audioSettings.pitch;
        _audioSource.spatialBlend = _audioSettings.spatialBlend;
        _audioSource.dopplerLevel = _audioSettings.dopplerLevel;
        _audioSource.minDistance = _audioSettings.minRange;
        _audioSource.maxDistance = _audioSettings.maxRange;
    }

    public void OnPlay()
    {
        _audioSource.Play();
    }


}
