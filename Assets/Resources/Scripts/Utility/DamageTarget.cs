using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : MonoBehaviour, ICanDamage
{
    public int iDamage;
    //public Collider dmgColi;
    public bool bIsProjectileAttack;
    public bool bIsTypeOfEnemy = true;
    public float fKnockForce = 4f;

    Vector3 startPos;

    public void Start()
    {
        if (bIsProjectileAttack)
        {
            startPos = transform.position;
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
            //if (other.gameObject.CompareTag("Player"))

            if (bIsTypeOfEnemy)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    IHittable _hitTarget = other.gameObject.GetComponent<IHittable>();
                    if (bIsProjectileAttack)
                    {
                        _hitTarget.Knockback(startPos, fKnockForce);
                    }
                    else
                    {
                        _hitTarget.Knockback(transform.position, fKnockForce);
                    }
                    _hitTarget.TakeDamage(Damage());
                }
                if (other.gameObject.CompareTag("Shield"))
                {
                    IHittable _hitTarget = other.gameObject.GetComponentInParent<IHittable>();
                    if (bIsProjectileAttack)
                    {
                        _hitTarget.Knockback(startPos, fKnockForce / 2f);
                    }
                    else
                    {
                        _hitTarget.Knockback(transform.position, fKnockForce / 2f);
                    }
                    _hitTarget.TakeDamage(0);
                }
            }
            else 
            {
                if (other.gameObject.CompareTag("Shield"))
                {
                    IHittable _hitTarget = other.gameObject.GetComponentInParent<IHittable>();
                    if (bIsProjectileAttack)
                    {
                        _hitTarget.Knockback(startPos, fKnockForce / 2f);
                    }
                    else
                    {
                        _hitTarget.Knockback(transform.position, fKnockForce / 2f);
                    }
                    _hitTarget.TakeDamage(0);
                }
                else
                {
                    IHittable _hitTarget = other.gameObject.GetComponent<IHittable>();

                    if(_hitTarget != null)
                    {
                        if (bIsProjectileAttack)
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
            }
        }
    }
}
