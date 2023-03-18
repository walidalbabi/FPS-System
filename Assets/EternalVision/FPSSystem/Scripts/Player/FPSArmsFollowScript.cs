using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSArmsFollowScript : MonoBehaviour
{

    private BodyConfiguration _bodyConfiguration;

    private void Awake()
    {
        _bodyConfiguration = GetComponentInParent<BodyConfiguration>();
    }

   

    // Update is called once per frame
    void Update()
    {
        transform.position = _bodyConfiguration.fullBodyArmsMatcher.position;
        transform.forward = _bodyConfiguration.fullBodyArmsMatcher.forward;
    }
}
