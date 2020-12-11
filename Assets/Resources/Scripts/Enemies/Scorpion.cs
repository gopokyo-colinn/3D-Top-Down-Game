using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Enemy, IHittable
{
    const float fTARGET_FOLLOW_RANGE = 60f;
    const float fPUSHBACKFORCE = 300f;
    private bool bRotateAnims = true;
    float fINVULNERABILITY_TIME = 1.2f;
    float fInvulnerableCounter;

    void Start()
    {
        base.Initialize();
        fAttackRange = 2f;
    }

    // Update is called once per frame
    private void Update()
    {
        /////// je attack krke out of range bhj jiye ta random move khrab ho janda
        base.Refresh();

       // Debug.Log(transform.name + " : " + bCanAttack);
        //Debug.Log(transform.name + " : " + bCanMove);
        //Debug.Log(transform.name + " : " + bCanRotate);
       // Debug.Log(transform.name + " : " + bCanFollow);
       /// Debug.Log(transform.name + " : " + bIsInvulnerable);
        SetAnimations();

        if (bIsAlive)
        {
            if(HelperFunctions.Grounded(transform, 0.2f))
            {
                if (!bTargetFound)
                {
                    FindingTarget();
                }
                else
                {
                    CheckTargetInRange();
                }
            }
            CalculateInvulnerability();
        }
    }
    private void FixedUpdate()
    {
        base.FixedRefresh();
        if (bIsAlive)
        {
            if (HelperFunctions.Grounded(transform, 0.2f))
            {
                if (!bTargetFound)
                {
                    MovingRandomly();
                }
                else
                {
                    if (!bCanAttack)
                    {
                        FollowTarget();
                    }
                    else
                    {
                        AttackMove();
                    }
                }
            }
        }
    }

    public void AttackMove()
    {
        if(fAttackWaitTimeCounter <= 0)
        {
            bCanFollow = false;
            anim.SetTrigger("StabAttack");
            StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { fAttackWaitTimeCounter = fAttackWaitTime; bCanFollow = true; bCanRotate = true; }, false, anim.GetCurrentAnimatorStateInfo(0).length));
            fAttackWaitTimeCounter = fAttackWaitTime;
            bCanRotate = true;
        }
    }
    public void SetAnimations()
    {
        if (bIsAlive)
        {
            anim.SetFloat("isWalking", rbody.velocity.sqrMagnitude);

            if (bIsHit)
            {
                anim.SetTrigger("isHit");
                bIsHit = false;
            }

            if (bCanRotate)
            {
                if (bRotateAnims)
                {
                    anim.SetTrigger("tRotating");

                    bRotateAnims = false;

                    if (bCanFollow)
                        bCanRotate = false;
                }
            }
            else
            {
                bRotateAnims = true;
            }
        }
        else
        {
            anim.SetTrigger("isDead");
        }

    }
    public void TakeDamage(int _damage)
    {
        if (!bIsInvulnerable)
        {
            Knockback(targetPlayer.transform.position, fPUSHBACKFORCE);
            IsInvulnerable(true);
            bIsHit = true;
            bTargetFound = true;
            bCanFollow = false;
            fCurrentHitPoints -= _damage;
        }
        if (fCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    public void CalculateInvulnerability()
    {
        if (bIsInvulnerable)
        {
            fInvulnerableCounter += Time.deltaTime;
            if(fInvulnerableCounter >= fINVULNERABILITY_TIME)
            {
                IsInvulnerable(false); 
                bIsHit = false; 
                bCanFollow = true;
                fInvulnerableCounter = 0;
            }
        }
    }
    public void IsInvulnerable(bool _invulnerable)
    {
        bIsInvulnerable = _invulnerable;
    }
    public void CheckTargetInRange()
    {
        fAttackWaitTimeCounter -= Time.deltaTime;
        // Out of Range
        if ((transform.position - targetPlayer.transform.position).sqrMagnitude >= fTARGET_FOLLOW_RANGE)
        {
            if (!bIsInvulnerable && bTargetFound)
            {
                bCanAttack = false;
                bCanFollow = false;
                bTargetFound = false;
                bIsMoving = false;
                bCanRotate = false;
                bCanMove = true;
               //// StopAllCoroutines();
                StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { bIsMoving = true; bCanMove = b; }, false, fWaitTime));
            }
        }
        // In Attack Range
        else if ((transform.position - targetPlayer.transform.position).sqrMagnitude <= fAttackRange)
        {
            if (!bIsInvulnerable) 
            {
                rbody.velocity = Vector3.zero;// HelperFunctions.VectorZero(rbody);
                if (fAttackWaitTimeCounter <= 0)
                {
                    Vector3 dir = (targetPlayer.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(dir, transform.forward);
                    if (dot > 0.95f)
                    {
                        bCanAttack = true;
                    }
                    else
                    {
                        bCanAttack = false;
                        HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATE_SPEED / 3f);
                    }
                }
            }
           
        }
        // In Non Attack Range
        else
        {
            bCanAttack = false;
        }
    }
}
