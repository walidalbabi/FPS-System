using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine_ForcePlayback : StateMachineBehaviour
{
    [SerializeField] private bool _forceStart;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        //if (_forceStart)
        //{
        //    animator.StartPlayback();
        //}
        //else
        //{
        //    animator.StopPlayback();
        //}
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (_forceStart)
        {
            animator.StartPlayback();
        }
        else
        {
            animator.StopPlayback();
        }
    }
}
