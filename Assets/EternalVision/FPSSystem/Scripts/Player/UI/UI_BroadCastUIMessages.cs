using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BroadCastUIMessages : NetworkBehaviour
{

    [SerializeField] private UI_DeathLogScript _deathMessage;
    [SerializeField] private Transform _deathMessageContent;
    [SerializeField] private TextMeshProUGUI _score;

    private LocalPlayerData _localPlayer;
    private int _killsCounts;

    private void Awake()
    {
        NetworkAgentHealth.OnDeathBoradcastToAllPlayers += OnPlayerDie;
        PlayerSpawner.OnPlayerSpawned += GetLocalPlayer;
    }

    private void OnDestroy()
    {
        NetworkAgentHealth.OnDeathBoradcastToAllPlayers -= OnPlayerDie;
        PlayerSpawner.OnPlayerSpawned -= GetLocalPlayer;
    }


    private void GetLocalPlayer(GameObject objPlayer)
    {
        _localPlayer = objPlayer.GetComponent<LocalPlayerData>();
    }


    private void OnPlayerDie(PlayerHealth victimPlayer, PlayerHealth killerPlayer)
    {
        if (!base.IsServer) return;
        var victim = victimPlayer.GetComponent<LocalPlayerData>();
        var killer = killerPlayer.GetComponent<LocalPlayerData>();
        ObserversSetDeathMessage(killer, victim);


    }

    [ObserversRpc]
    private void ObserversSetDeathMessage(LocalPlayerData localPlayerDataKiller, LocalPlayerData localPlayerDataVictim)
    {
        if (base.IsServer) return;
            SetDeathMessage(localPlayerDataKiller, localPlayerDataVictim);

        if (_localPlayer == localPlayerDataKiller)
        {
            _killsCounts++;
            _score.text = "K/" + _killsCounts;
        }
    }


    private void SetDeathMessage(LocalPlayerData localPlayerDataKiller, LocalPlayerData localPlayerDataVictim)
    {
        var log = Instantiate(_deathMessage, _deathMessageContent);
        string name_1 = localPlayerDataKiller.playerName;
        string name_2 = localPlayerDataVictim.playerName;
        log.SetDeathLog(name_1, name_2, false);
    }


}
