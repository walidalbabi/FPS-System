using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    public static event Action<ObjectPool> OnPooledObjectsAdded;

    [Header("Pool Settings")]
    [SerializeField] private Pool_Bullets _poolBullets;
    [SerializeField] private Pool_Audio _poolAudio;

    [Header("Pool Data")]
    public List<Projectile> _bulletsList = new List<Projectile>();
    public List<GameObject> _bulletsImpactsList = new List<GameObject>();
    public List<PooledAudioObject> _audioList = new List<PooledAudioObject>();


    private bool isPoolObjectSettedUp;

    private void Awake()
    {

        PlayerController.OnPlayerAvailbleInScene += Init;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerAvailbleInScene -= Init;
    }


    private void Init(GameObject objPlayer)
    {
        if (objPlayer == null) Debug.LogError("Player Object null");
        if (isPoolObjectSettedUp) return;

        SetupGameBullets();
        SetupBulletImpacts();
        SetupGameAudio();

        OnStart();
     }

    private void OnStart()
    {
        OnPooledObjectsAdded?.Invoke(this);
        isPoolObjectSettedUp = true;
        Debug.Log("Pool Availble" + this != null ? "availble" : "Null");
    }

    private void SetupGameBullets()
    {
        for (int i = 0; i < _poolBullets.numberOfBulletsInGames; i++)
        {
            for (int j = 0; j < _poolBullets.bulletsPrefab.Length; j++)
            {
                Projectile obj = Instantiate(_poolBullets.bulletsPrefab[j]);
                _bulletsList.Add(obj);
                obj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void SetupBulletImpacts()
    {
        for (int j = 0; j < _poolBullets.surfaces.Length; j++)
        {
            for (int i = 0; i < _poolBullets.numberOfBulletsInGames; i++)
            {
                GameObject obj = Instantiate(_poolBullets.surfaces[j].bulletImpact);
                _bulletsImpactsList.Add(obj);
                obj.name = _poolBullets.surfaces[j].SurfaceName;
                obj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void SetupGameAudio()
    {
        for (int i = 0; i < _poolAudio.maxAudioInGame; i++)
        {
            PooledAudioObject obj = Instantiate(_poolAudio.audioPefab);
            _audioList.Add(obj);
            obj.OnInit(this);
           // obj.gameObject.hideFlags = HideFlags.HideInHierarchy;
            obj.gameObject.SetActive(false);
        }
    }

    //Projectiles
    public Projectile RetrievePoolBulletObject(Projectile projectile)
    {
        foreach (Projectile bullet in _bulletsList)
        {
            if (projectile.projectileType == bullet.projectileType)
            {
                if (!bullet.gameObject.activeInHierarchy)
                {
                    bullet.gameObject.SetActive(true);
                    _bulletsList.Remove(bullet);
                    return bullet;
                }
            }
        }
        Debug.Log("Pool bullets busy");
        return null;
    }

    public void SendBackToPool(Projectile bullet)
    {
        _bulletsList.Add(bullet);
        bullet.gameObject.SetActive(false);
    }

    //Impacts
    public GameObject RetrievePoolBulletImpactObject(SurfaceIdentifier surfaceData)
    {
        foreach (var surface in _bulletsImpactsList)
        {
            if (!surface.gameObject.activeInHierarchy)
            {
                if (surfaceData.surfaceData.SurfaceName == surface.name)
                {
                    surface.gameObject.SetActive(true);
                    _bulletsImpactsList.Remove(surface);
                    return surface.gameObject;
                }
            }
        }
        Debug.Log("Pool bullets busy");
        return null;
    }

    public void SendBackToPool(GameObject bulletImpact)
    {
        _bulletsImpactsList.Add(bulletImpact);
        bulletImpact.gameObject.SetActive(false);
    }

    //Audio
    public PooledAudioObject RetrievePoolAudio()
    {
        foreach (PooledAudioObject audio in _audioList)
        {
            if (!audio.gameObject.activeInHierarchy)
            {
                audio.gameObject.SetActive(true);
                _audioList.Remove(audio);
                return audio;
            }

        }
        Debug.Log("Pool Audio busy");
        return null;
    }
    public void SendBackToPool(PooledAudioObject audio)
    {
        _audioList.Add(audio);
        audio.gameObject.SetActive(false);
    }

}

[System.Serializable]
public struct Pool_Bullets
{
    public Projectile[] bulletsPrefab;
    public int numberOfBulletsInGames;
    public SO_SurfaceData[] surfaces;
}

[System.Serializable]
public struct Pool_Audio
{
    public PooledAudioObject audioPefab;
    public float maxAudioInGame;
}