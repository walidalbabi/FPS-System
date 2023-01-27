using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMovementsAnimationStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("LockMovements", true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("LockMovements", false);
    }
}
