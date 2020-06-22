using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart { spine, leftHand }

public class PlayerAnimationFunctions : MonoBehaviour
{
    PlayerController player;
    public Transform spine;
    public Transform leftHand;
    public GameObject shield;
    Animator anim;
    AvatarMask mask;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();
        Debug.Log("worked");
        MaskSwitch();
    }
    void ShieldParentCheck()
    {
        //if (!player.isShielding)
        //    shield.transform.parent = spine.transform;
        //else
        //    shield.transform.parent = leftHand.transform;
    }

    public void ShieldOnBodyPart(BodyPart bodyPart)
    {
        if(bodyPart == BodyPart.spine)
            shield.transform.parent = spine.transform;
        else if(bodyPart == BodyPart.leftHand)
            shield.transform.parent = leftHand.transform;
    }

    public void ShielOnSpine()
    {
        shield.transform.parent = spine.transform;

    }
    public void ShieldInHand()
    {
        shield.transform.parent = leftHand.transform;
    }

    public void MaskSwitch()
    {
        Debug.Log(anim.GetLayerName(3));
    }

    
}
