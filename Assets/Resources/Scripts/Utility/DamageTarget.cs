using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : MonoBehaviour, ICanDamage
{
    public int iDamage;
    public Collider dmgColi;
    public bool isProjectileAttack;
    public float fKnockForce = 4f;

    Vector3 startPos;

    public void Start()
    {
        if (isProjectileAttack)
        {
            startPos = transform.position;
            Debug.Log("I am projectile");
        }
    }
    public int Damage()
    {
        return iDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(other.gameObject.GetComponent<IHittable>() != null)
                {
                    IHittable _hitTarget = other.gameObject.GetComponent<IHittable>();
                    if (isProjectileAttack)
                    {
                        _hitTarget.Knockback(startPos, fKnockForce);
                    }
                    else
                    {
                        _hitTarget.Knockback(transform.position, fKnockForce);
                    }
                    _hitTarget.TakeDamage(Damage());
                }
            }
            else
            {
                if (other.gameObject.CompareTag("Shield"))
                {
                    Debug.Log("Attack Blocked");
                    if(dmgColi != null)
                        dmgColi.gameObject.SetActive(false);
                }
            }
        }
    }
}
