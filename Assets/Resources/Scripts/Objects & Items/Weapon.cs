using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, ICanDamage
{
    public int iDamage;
    public float fKnockBackForce;
    Collider coli;
    PlayerController player;


    public void Start()
    {
        coli = GetComponent<Collider>();
        coli.enabled = false;
        player = GetComponentInParent<PlayerController>();
    }

    public void Update()
    {
        CheckPlayerForAttack();
    }
    public int Damage()
    {
        return iDamage;
    }
    public void CheckPlayerForAttack()
    {
        if (player.bIsAttacking)
        {
           // trialEffectAnimator.SetTrigger("slash_1");
            coli.enabled = true;
        }
        else
        {
            coli.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            if(other.gameObject.GetComponent<IHittable>() != null)
            {
                IHittable _hitTarget = other.gameObject.GetComponent<IHittable>();
                _hitTarget.Knockback(transform.position, fKnockBackForce);
                _hitTarget.TakeDamage(iDamage);
            }
        }
    }
}