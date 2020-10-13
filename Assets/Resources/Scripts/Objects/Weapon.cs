using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, ICanDamage
{
    public int damage;
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
        return damage;
    }
    public void CheckPlayerForAttack()
    {
        if (player.isAttacking)
        {
            coli.enabled = true;
        }
        else
        {
            coli.enabled = false;
        }
    }
}
