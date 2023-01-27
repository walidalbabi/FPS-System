using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledAudioObject : MonoBehaviour
{

    private AudioSource _audioSource;
    private ObjectPool _objectPool;
    private AudioSettings _audioSettings;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioSettings(AudioSettings settings, AudioClip clip)
    {
        _audioSettings = settings;
        _audioSource.clip = clip;
        _audioSource.volume = _audioSettings.volume;
        _audioSource.pitch = _audioSettings.pitch;
        _audioSource.spatialBlend = _audioSettings.spatialBlend;
        _audioSource.dopplerLevel = _audioSettings.dopplerLevel;
        _audioSource.minDistance = _audioSettings.minRange;
        _audioSource.maxDistance = _audioSettings.maxRange;
    }

    public void Play()
    {
        StartCoroutine(ReturnToPoolAfterDelay(_audioSource.clip.length));
        _audioSource.Play();
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.parent = null;
        _objectPool.SendBackToPool(this);
    }


    public void OnInit(ObjectPool component)
    {
        _objectPool = component;
    }
}
