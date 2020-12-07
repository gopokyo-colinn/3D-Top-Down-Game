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
    private void Update()
    {
        base.Refresh();
        if (!bTargetFound)
        {
            FindingTarget();
        }
    }
    void FixedUpdate()
    {
        base.FixedRefresh();

        if (!bTargetFound)
        {
            MovingRandomly();
        }
        else
        {
            FollowTarget();
        }
    }
}
