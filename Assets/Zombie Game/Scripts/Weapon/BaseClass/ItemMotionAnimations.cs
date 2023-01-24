using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMotionAnimations : MonoBehaviour
{
    private PlayerItem _playerItem;

    private void Awake()
    {
        _playerItem = GetComponent<PlayerItem>();
    }


    public void SetRun(bool isRun)
    {
        foreach (var anim in _playerItem.animators)
        {
            anim.SetBool("Run", isRun);
        }
    }

    public void SetJump()
    {
        foreach (var anim in _playerItem.animators)
        {
            anim.SetTrigger("Jump");
        }
    }

    public void SetFall(bool isFall)
    {
        foreach (var anim in _playerItem.animators)
        {
            anim.SetBool("Falling", isFall);
        }
    }
}
