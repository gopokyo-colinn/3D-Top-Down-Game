using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Collider coli;
    PlayerController player;
    public ParticleSystem weaponTrialEffect;

    //TODO: Add functionality to be used by enemies too...

    public void Start()
    {
        coli = GetComponent<Collider>();
        coli.enabled = false;
        player = PlayerController.Instance;
    }

    public void Update()
    {
        if (player.IsAttacking())
        {
            coli.enabled = true;
            StartCoroutine(HelpUtils.ChangeBoolAfter((bool b)=> { coli.enabled = b; }, false, player.GetAnimator().GetCurrentAnimatorStateInfo(player.GetAnimator().GetLayerIndex("SwordAnims(Right Hand)")).length));
        }
    }

    public void CheckPlayerForAttack()
    {
        if (player.IsAttacking())
        {
            coli.enabled = true;
        }
        else
        {
            coli.enabled = false;
        }
    }

}