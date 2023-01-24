using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Deathmatch : GameMode
{
    public override void Update()
    {
        base.Update();
    }


    public override void OnPlayerDie()
    {
        _startSpawn = true;
    }

}
