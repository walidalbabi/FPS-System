using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] public string projectileType;
    [SerializeField] protected float _projectileSpeed = 5f;

    protected Vector3 _startPos;
    protected Vector3 _endPos;
    protected float distanceToEnd;

    protected ObjectPool _objectPool;
    protected SurfaceIdentifier _surface;


    public virtual void Awake()
    {
        _objectPool = GameManager.instance.objPool;
    }

    public virtual void Update()
    {
        if (_objectPool == null) return;

        distanceToEnd = Vector3.Distance(transform.position, _endPos);

        if (transform.position != _endPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endPos, Time.deltaTime * _projectileSpeed);
        }
        else
        {
            //Send Projectile Backto Pool
            SendBackToPool();
        }
    }

    public virtual void AddProjectile(Vector3 startPos, Vector3 endPos, SurfaceIdentifier surface)
    {
        ResetProjectile();
        transform.position = startPos;
        transform.rotation = Quaternion.identity;

        _startPos = startPos;
        _endPos = endPos;
        _surface = surface;
    }

    public virtual void AddProjectile(Vector3 postion, Vector3 direction, float passedTime)
    {
        ResetProjectile();
    }

    public virtual void ResetProjectile()
    {
        _startPos = Vector3.zero;
        _endPos = Vector3.zero;
        _surface = null;
    }

    public void SendBackToPool()
    {
        _objectPool.SendBackToPool(this);
    }

}
