using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Enemy
{
    private bool bRotateAnims = true;
    float fSTUN_TIME = 0f; // this is extra time after the animation

    public Transform[] travelPoints;
    void Start()
    {
        base.Initialize();
        fAttackRange = 2.5f;
        fFollowRange = 120f;
    }

    // Update is called once per frame
    private void Update()
    {
        base.Refresh();

        SetAnimations();

        if (bIsAlive)
        {
            if(bIsGrounded)
            {
                if (!bTargetFound)
                {
                    FindingTarget();
                }
                else
                {
                    CheckTargetInRange(fAttackRange, fFollowRange);
                }
            }
            CalculateInvulnerability(fSTUN_TIME);
        }
    }
    private void FixedUpdate()
    {
        base.FixedRefresh();
        if (bIsAlive)
        {
            if (bIsGrounded)
            {
                if (!bTargetFound)
                {
                    MoveRandomly();
                }
                else
                {
                    if (!bCanAttack)
                    {
                        if (bCanFollow)
                        {
                            FollowTarget(targetPlayer.transform.position);
                        }
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
            bIsAttacking = true;
            bCanRotate = false;
            StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsAttacking = false; fAttackWaitTimeCounter = fAttackWaitTime; bCanFollow = true; bCanRotate = true; }, false, anim.GetCurrentAnimatorStateInfo(0).length + 0.5f));
            fAttackWaitTimeCounter = fAttackWaitTime;
            //bCanRotate = true;
        }
    }
    public void SetAnimations()
    {
        if (bIsAlive)
        {
            float f = bIsMoving ? 1 : 0;
            anim.SetFloat("isWalking", f);
            anim.SetBool("canAttack", bCanAttack);

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
  
}
