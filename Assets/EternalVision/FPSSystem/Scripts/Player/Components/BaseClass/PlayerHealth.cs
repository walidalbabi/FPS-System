using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    #region Public.
    /// <summary>
    /// Dispatched when health changes with old, new, and max health values.
    /// </summary>
    public event Action<int, int, int> OnHealthChanged;
    /// <summary>
    /// Called When damage is Already Substracted only if we didn't die
    /// </summary>
    public event Action<int> OnDamageTaken;
    /// <summary>
    /// Dispatched when health is depleted.
    /// </summary>
    public event Action OnDeath;
    /// <summary>
    /// Dispatched after being respawned.
    /// </summary>
    public event Action OnRespawned;
    /// <summary>
    /// Current health.
    /// </summary>
    public int CurrentHealth { get; protected set; }
    /// <summary>
    /// Maximum amount of health character can currently achieve.
    /// </summary>
    public int MaximumHealth { get { return _baseHealth; } }

    /// <summary>
    /// Hitboxes on the character.
    /// </summary>
    public Hitbox[] Hitboxes { get; private set; } = new Hitbox[0];
    #endregion

    #region Serialized.
    /// <summary>
    /// Health to start with.
    /// </summary>
    [Tooltip("Health to start with.")]
    [SerializeField]
    private int _baseHealth = 100;


    #endregion

    //Private
    private PlayerMovements _playerMovements;
    private LocalPlayerData _localPlayerData;

    //Protected
    protected int _oldHealth;


    public virtual void Awake()
    {
        _playerMovements = GetComponent<PlayerMovements>();
        _localPlayerData = GetComponent<LocalPlayerData>();

         CurrentHealth = MaximumHealth;

       // SetFallDamageEvent();
        // SetHitboxes();
    }

    /// <summary>
    /// Finds hitboxes on this object.
    /// </summary>
    public virtual void SetHitboxes()
    {
        Hitboxes = GetComponentsInChildren<Hitbox>();
        for (int i = 0; i < Hitboxes.Length; i++)
        {
            Hitboxes[i].OnHit += Health_OnHit;
            Hitboxes[i].SetTopmostParent(transform);
            Hitboxes[i].SetOwnerHash(gameObject.GetHashCode());
        }
    }

    public void SetFallDamageEvent()
    {
        _playerMovements.OnFallDamageTaken += Health_OnFall;
    }

    /// <summary>
    /// Received when a hitbox is hit.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    public virtual void Health_OnHit(Hitbox hitbox, int damage, GameObject playerDealer)
    {
        RemoveHealth(damage, hitbox.Multiplier);
    }

    /// <summary>
    /// On Player Fall Take Damage
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Health_OnFall(int damage)
    {
        RemoveHealth(damage ,1);
    }

    /// <summary>
    /// Restores health to maximum health.
    /// </summary>
    public virtual void RestoreHealth()
    {
        int oldHealth = CurrentHealth;
        CurrentHealth = MaximumHealth;

        OnHealthChanged?.Invoke(oldHealth, CurrentHealth, MaximumHealth);
    }

    /// <summary>
    /// Called when respawned.
    /// </summary>
    public virtual void Respawned()
    {
        OnRespawned?.Invoke();
        if (_localPlayerData.onLadder) _playerMovements.ForceExitPlayerLadder();
    }

    /// <summary>
    /// Removes health.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="multiplier"></param>
    public void RemoveHealth(int value, float multiplier)
    {
        RemoveHealth(Mathf.CeilToInt(value * multiplier));
    }

    /// <summary>
    /// Removes health.
    /// </summary>
    /// <param name="value"></param>
    public virtual void RemoveHealth(int value)
    {
        _oldHealth = CurrentHealth;
        CurrentHealth -= value;

        OnHealthChanged?.Invoke(_oldHealth, CurrentHealth, MaximumHealth);

        if (CurrentHealth <= 0f)
            HealthDepleted();
        else OnDamageTaken?.Invoke(value);
    }

    /// <summary>
    /// Called when health is depleted.
    /// </summary>
    public virtual void HealthDepleted()
    {
        OnDeath?.Invoke();
    }
}
