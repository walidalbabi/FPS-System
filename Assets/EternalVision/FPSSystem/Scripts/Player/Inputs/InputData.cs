using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputData 
{
    public Vector2 moveInputs;
    public Vector2 viewRotation;
    public Vector2 itemScroll;
    /// <summary>
    /// Y rotation of player.
    /// </summary>
    public float Rotation;
    public bool jump;
    public bool sprintHold;
    public bool stealthWalk;
    public bool crouch;
    public bool LeftClick;
    public bool RightClick;
    public bool reload;
    public bool interact;
}

public struct ReconcileData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 verticalVelocity;
    public Vector3 movementInputs;
    public float moveSpeed;
    public float stamina;
}
