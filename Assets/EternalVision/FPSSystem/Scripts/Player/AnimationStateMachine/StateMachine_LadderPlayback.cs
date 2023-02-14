using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine_LadderPlayback : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // this is counter intuitive to how I think it should work. 
        if (animator.GetFloat("Vertical") > 0.1f || animator.GetFloat("Vertical") < -0.1f)
        {
            animator.StopPlayback();
        }
        else
        {
            animator.StartPlayback();
        }
    }

}
