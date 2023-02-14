using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderExitTrigger : MonoBehaviour
{
    private enum ExitType{
        QuickExit,
        AnimatedExit
    }

    [SerializeField] private ExitType _exitType;
    [SerializeField] private Vector3 _exitOffset;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovements playerMovement = other.GetComponent<PlayerMovements>();

        if(playerMovement != null)
        {
            if (!playerMovement.GetComponent<LocalPlayerData>().onLadder) return;

            if (_exitType == ExitType.QuickExit)
                playerMovement.ForceExitPlayerLadder();

            if (_exitType == ExitType.AnimatedExit)
                playerMovement.AnimatedExitPlayerLadder(transform.parent.right * -1f, transform.position + _exitOffset);

        }
    }
}
