using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioSettings
{
    [Range(0f, 1f)]public float volume;
    [Range(-3f, 3f)] public float pitch;
    [Range(0, 1f)] public float spatialBlend;
    [Range(0, 1f)] public float dopplerLevel;
    [Range(0f, 1f)] public float minRange;
    [Range(0f, 10000f)] public float maxRange;
}
