using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturrnToPoolAfterDelay : MonoBehaviour
{
    [SerializeField] private float _time;

    private ObjectPool _objectPool;

    private void Awake()
    {
        _objectPool = GameManager.instance.objPool;
    }

    private void OnEnable()
    {
        StartCoroutine(ReturnToPool(_time));
    }

    IEnumerator ReturnToPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        _objectPool.SendBackToPool(this.gameObject);
    }

    private void GetPooledObjectReference(ObjectPool component)
    {
        _objectPool = component;
    }
}
