using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] protected GameObject _playerAgent;
    [SerializeField] protected bool _canSpawn = true;

    [Header("Runtime")]
    [SerializeField] protected GameObject _spawnedAgent;

    public static event Action<GameObject> OnPlayerSpawned;



    /// <summary>
    /// On this object Start , can be called from whatever starting function
    /// </summary>
    public virtual void SpawnPlayer()
    {
        if (!_canSpawn) return;

        _spawnedAgent = Instantiate(_playerAgent);
        PlayerSpawned(_spawnedAgent);
    }

    public virtual void PlayerSpawned(GameObject obj)
    {
        OnPlayerSpawned?.Invoke(obj);
    }

}