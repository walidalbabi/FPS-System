using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : PlayerSpawner
{

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner) GameManager.instance.networkContext.CreateOwnership(base.IsOwner, base.IsClient, base.IsServer, base.IsHost);
    }



    private void Awake()
    {
        GameMode.SpawnPlayer += StartSpawner;
    }

    private void OnDisable()
    {
        GameMode.SpawnPlayer -= StartSpawner;
    }

    private void StartSpawner()
    {
        if (base.IsOwner)
        {
            SpawnPlayer();
        }
    }

    [Client(Logging = LoggingType.Off)]
    public override void SpawnPlayer()
    {
        if (!_canSpawn) return;
        if (_playerAgent != null)
            SpawnNetworkPlayerAgent(_playerAgent);
    }

    [ServerRpc]
    private void SpawnNetworkPlayerAgent(GameObject obj)
    {
        Transform spawnPos = GameManager.instance.GetSpawnPoint();
        if(_spawnedAgent == null)
        {
            _spawnedAgent = Instantiate(obj, spawnPos.position, Quaternion.identity);
            base.Spawn(_spawnedAgent, base.Owner);
            CallOnPlayerSpawnedEvent(base.Owner, _spawnedAgent.GetComponent<NetworkObject>());
            SetSpawnedPlayerRefference(_spawnedAgent);
        }
        else
        {
            _spawnedAgent.transform.position = spawnPos.position;
            _spawnedAgent.transform.rotation = Quaternion.Euler(0f, spawnPos.eulerAngles.y, 0f);
            Physics.SyncTransforms();
            //Restore health and set respawned.
            _spawnedAgent.GetComponent<PlayerHealth>().RestoreHealth();
            _spawnedAgent.GetComponent<PlayerHealth>().Respawned();
        }
    }

    [TargetRpc]
    private void CallOnPlayerSpawnedEvent(NetworkConnection conn, NetworkObject character)
    {
        GameObject playerObj = character.gameObject;
        if (playerObj == null) return;
        PlayerSpawned(playerObj);
    }

    [ObserversRpc]
    private void SetSpawnedPlayerRefference(GameObject character)
    {
        if (!base.IsOwner)
            _spawnedAgent = character;
    }


}
