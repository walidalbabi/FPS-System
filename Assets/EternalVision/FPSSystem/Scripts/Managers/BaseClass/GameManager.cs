using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private ObjectPool _objPool;
    [SerializeField] private NetworkContext _networkContext;

    private int _currentSpawnPoint = 0;


    public ObjectPool objPool { get { return _objPool; } }
    public NetworkContext networkContext { get { return _networkContext; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public Transform GetSpawnPoint()
    {
        _currentSpawnPoint = _currentSpawnPoint < (_spawnPoints.Length - 1) ? _currentSpawnPoint = _currentSpawnPoint + 1 : 0;
        return _spawnPoints[_currentSpawnPoint];
    }

}
