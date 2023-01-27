using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceData", menuName = "EV/Surface", order = 1)]
public class SO_SurfaceData : ScriptableObject
{
    public string SurfaceName;
    public GameObject bulletImpact;
    public AudioClip[] footSteps;
    public AudioClip[] landSounds;
    public AudioClip[] fallImpactsSounds;
}
