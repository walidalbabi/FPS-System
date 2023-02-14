using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine_RigStrenght : StateMachineBehaviour
{
    [SerializeField] private float _strenghtOnEnter;
    [SerializeField] private float _strenghtOnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<IKControl>().SetPlayerRigStrenght(_strenghtOnEnter);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<IKControl>().SetPlayerRigStrenght(_strenghtOnExit);
    }
}
