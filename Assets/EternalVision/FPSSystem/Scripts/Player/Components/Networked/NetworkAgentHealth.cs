
using EternalVision.FPS;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

public class NetworkAgentHealth : PlayerHealth
{
    /// <summary>
    /// When a player die send a message with victim, killer data
    /// </summary>
    public static event Action<PlayerHealth, PlayerHealth> OnDeathBoradcastToAllPlayers;

    public bool isDead;

    [HideInInspector][SyncVar]
    public GameObject playerKillerOfThisComponent;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetHitboxes();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SetHitboxes();
        SetFallDamageEvent();
    }


    public override void Health_OnHit(Hitbox hitbox, int damage, GameObject playerDealer)
    {
        if (isDead) return;
        playerKillerOfThisComponent = playerDealer;
        base.Health_OnHit(hitbox, damage, playerDealer);
    }

    public override void Health_OnFall(int damage)
    {
        if (isDead) return;
        playerKillerOfThisComponent  = null;
        base.Health_OnFall(damage);
    }

    public override void RestoreHealth()
    {
        base.RestoreHealth();

        if (GameManager.instance.networkContext.Ownership.isServer)
            ObserversRestoreHealth();
    }

    public override void Respawned()
    {
        base.Respawned();
        isDead = false;

        if (GameManager.instance.networkContext.Ownership.isServer)
            ObserversRespawned();
    }

    public override void RemoveHealth(int value)
    {
        base.RemoveHealth(value);


        if (GameManager.instance.networkContext.Ownership.isServer)
            ObserversRemoveHealth(value, _oldHealth);

        Debug.Log("<color=cyan> Player Health : </color> " + CurrentHealth);
    }

    public override void HealthDepleted()
    {
        base.HealthDepleted();
        CallPlayerDiedBroadcastMessage();
        isDead = true;
    }


 
    private void CallPlayerDiedBroadcastMessage()
    {
        if (playerKillerOfThisComponent != null)
            OnDeathBoradcastToAllPlayers?.Invoke(this, playerKillerOfThisComponent.GetComponent<PlayerHealth>());

        playerKillerOfThisComponent = null;
    }

    /// <summary>
    /// Sent to clients when health is restored.
    /// </summary>
    [ObserversRpc]
    private void ObserversRestoreHealth()
    {
        //Server already restored health. If we don't exit this will be an endless loop. This is for client host.
        if (GameManager.instance.networkContext.Ownership.isServer)
            return;

        RestoreHealth();
    }

    /// <summary>
    /// Sent to clients when character is respawned.
    /// </summary>
    [ObserversRpc]
    private void ObserversRespawned()
    {
        if (GameManager.instance.networkContext.Ownership.isServer)
            return;

        Respawned();
    }


    /// <summary>
    /// Sent to clients to remove a portion of health.
    /// </summary>
    /// <param name="value"></param>
    [ObserversRpc]
    private void ObserversRemoveHealth(int value, int priorHealth)
    {
        //Server already removed health. If we don't exit this will be an endless loop. This is for client host.
        if (GameManager.instance.networkContext.Ownership.isServer)
            return;

        /* Set current health to prior health so that
         * in case client somehow magically got out of sync
         * this will fix it before trying to remove health. */
        CurrentHealth = priorHealth;

        RemoveHealth(value);
    }

}
