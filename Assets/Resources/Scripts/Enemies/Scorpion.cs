using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Enemy
{
    void Start()
    {
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        base.Refresh();
        if(!bTargetFound)
            MovingRandomly();
        else
            FollowTarget();
    }
}
