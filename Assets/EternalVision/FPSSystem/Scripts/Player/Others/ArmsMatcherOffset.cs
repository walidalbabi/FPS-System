using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsMatcherOffset : MonoBehaviour
{

    [SerializeField] private Vector3 _upOffset;
    [SerializeField] private Vector3 _downOffset;

    private Vector3 _startLocalPos;
    private Vector3 _offsetPos;
    private PlayerMovements _playerMovements;


    private void Awake()
    {
        _playerMovements = GetComponentInParent<PlayerMovements>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _startLocalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateOffsetPosition();
        transform.localPosition = _startLocalPos + _offsetPos;
    }


    private void CalculateOffsetPosition()
    {
        var angle = _playerMovements.lookDirection.x;
        angle /= 90f * -1f;

        if (angle > 0) _offsetPos = _upOffset * angle;
        else if (angle < 0) _offsetPos = _downOffset * angle;
        else _offsetPos = Vector3.zero;      
    }
}
