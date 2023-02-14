using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : NetworkBehaviour, I_Interactable
{

    [SerializeField] private Transform _ladderTop;
    [SerializeField] private Transform _ladderBottom;

    [SyncVar(OnChange = nameof(OnChangePlayer))]
    public GameObject playerObject;


    private Vector3 _startPos;


    public void Interact(GameObject playerObject)
    {
        this.playerObject = playerObject;
        float distanceA = Vector3.Distance(_ladderBottom.position, playerObject.transform.position);
        float distanceB = Vector3.Distance(_ladderTop.position, playerObject.transform.position);

        _startPos = distanceA < distanceB ? _ladderBottom.position : _ladderTop.position;
    }

    private void OnChangePlayer(GameObject oldValue, GameObject newValue, bool asServer)
    {
        if (newValue == null) return;
        TargetSetPlayerOnLadder(playerObject.GetComponent<PlayerMovements>().Owner);
        playerObject = null;
    }

    [TargetRpc]
    private void TargetSetPlayerOnLadder(NetworkConnection connection)
    {
        if(playerObject == null) Debug.Log("Null Player");

        playerObject.GetComponent<PlayerMovements>().SetPlayerLadder(transform.right * -1f, _startPos, this);
        Debug.Log("On Ladder");
    }
}
