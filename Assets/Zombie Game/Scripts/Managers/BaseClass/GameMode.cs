using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class GameMode : NetworkBehaviour
{

    [SerializeField] protected float _maxTimeToRespawn;

    protected PlayerHealth _playerHealth;
    protected float _timeToRespawn;
    protected bool _startSpawn;


    public static event Action SpawnPlayer;

    public virtual void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += GetHealthComponentOfCurrentPlayer;
    }

    public virtual void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= GetHealthComponentOfCurrentPlayer;

        if(_playerHealth)
            _playerHealth.OnDeath -= OnPlayerDie;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _startSpawn = true;
    }

    public virtual void Update()
    {
        if (_startSpawn)
        {
            if (_timeToRespawn < _maxTimeToRespawn)
            {
                _timeToRespawn += Time.deltaTime;
            }
            else
            {
                _timeToRespawn = 0;
                _startSpawn = false;
                SpawnPlayer?.Invoke();
            }
        }
    }

    private void GetHealthComponentOfCurrentPlayer(GameObject objPlayer)
    {
        _playerHealth = objPlayer.GetComponent<PlayerHealth>();
        _playerHealth.OnDeath += OnPlayerDie;
    }

    public abstract void OnPlayerDie();

}
