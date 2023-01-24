using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBodyAnimationEventsHandler : MonoBehaviour
{
    public event Action<string> OnPlayerFootSteps;

    public void OnPlayFootsteps(string state)
    {
        OnPlayerFootSteps?.Invoke(state);
    }
}
