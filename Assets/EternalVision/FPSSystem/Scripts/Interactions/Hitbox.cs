using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{


    private int _ownerHash;

    /// <summary>
    /// Dispatched when this hitbox is hit
    /// </summary>
    public event Action<Hitbox, int, GameObject> OnHit;
    /// <summary>
    /// Topmost parent of this hitbox.
    /// </summary>
    public Transform TopmostParent { get; private set; }
    /// <summary>
    /// Sets the TopmostParent value.
    /// </summary>
    /// <param name="t"></param>

    public int ownerHash { get { return _ownerHash; } }


    public void SetTopmostParent(Transform t)
    {
        TopmostParent = t;
    }

    public void SetOwnerHash(int hash)
    {
        _ownerHash = hash;
    }

    #region Serialized.
    /// <summary>
    /// 
    /// </summary>
    [Tooltip("Amount of multiplier to apply towards normal damage when this hitbox is hit.")]
    [SerializeField]
    private float _multiplier = 1f;
    /// <summary>
    /// Amount of multiplier to apply towards normal damage when this hitbox is hit.
    /// </summary>
    public float Multiplier { get { return _multiplier; } }
    #endregion

    /// <summary>
    /// Indicates a hit to this hitbox.
    /// </summary>
    /// <param name="damage">Amount of damage from hit.</param>
    public void Hit(int damage, GameObject dealer)
    {
        OnHit?.Invoke(this, damage, dealer);
    }

    private CapsuleCollider coll;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider>();
    }

    private void OnDrawGizmos()
    {
        if (coll == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, coll.radius);
    }
}
