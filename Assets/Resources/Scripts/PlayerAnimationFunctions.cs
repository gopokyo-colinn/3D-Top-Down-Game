using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationFunctions : MonoBehaviour
{
    PlayerController player;
    public Transform spine;
    public Transform leftHand;
    public GameObject shield;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }
    void ShieldParentCheck()
    {
        //if (!player.isShielding)
        //    shield.transform.parent = spine.transform;
        //else
        //    shield.transform.parent = leftHand.transform;
    }
}
