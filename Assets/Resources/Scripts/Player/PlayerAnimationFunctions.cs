using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPartToAttachTo { spine, leftHand, rightHand }

public class PlayerAnimationFunctions : MonoBehaviour
{
    PlayerController player;

    public Transform shieldDefault;
    public Transform swordDefault;
    public Transform shieldHandParent;
    public Transform swordHandParent;

    public GameObject shield;
    public GameObject sword;

    public GameObject trialEffects;
    Animator trialEffectAnimator;
    GameObject swish1;

    //public GameObject swordSlashParticles;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        trialEffectAnimator = trialEffects.GetComponent<Animator>();
        swish1 = trialEffects.transform.GetChild(0).gameObject;
        swish1.SetActive(false);
    }
    public void ShieldActivate(BodyPartToAttachTo bodyPart)
    {
        if(bodyPart == BodyPartToAttachTo.spine)
            shield.transform.parent = shieldDefault.transform;
        else if(bodyPart == BodyPartToAttachTo.leftHand)
            shield.transform.parent = shieldHandParent.transform;

        shield.transform.localPosition = Vector3.zero;
        shield.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void SwordActivate(BodyPartToAttachTo bodyPart)
    {
        if(bodyPart == BodyPartToAttachTo.spine)
            sword.transform.parent = swordDefault.transform;
        else if(bodyPart == BodyPartToAttachTo.rightHand)
            sword.transform.parent = swordHandParent.transform;

        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void EnableSlashParticles()
    {
        trialEffectAnimator.SetBool("slash_1b", player.bIsAttacking);
    }
    public void DisableSlashParticles()
    {
        trialEffectAnimator.SetBool("slash_1b", false);
    }
    public void SetAttackFalse()
    {
       player.bCanAttack = true;
    }

    IEnumerator disableGameObjectAfter(GameObject _go, bool _enableDisable)
    {
        yield return new WaitForSeconds(.2f);
        _go.SetActive(_enableDisable);
    }

    
}
