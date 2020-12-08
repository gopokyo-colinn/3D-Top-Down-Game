using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Enemy
{
    private bool bRotateAnims = true;
    
    void Start()
    {
        base.Initialize();
        fAttackRange = 2f;
        fINVULNERABILITY_TIME = 0.5f;
    }

    // Update is called once per frame
    private void Update()
    {
        base.Refresh();
        SetAnimations();

        if (bIsAlive)
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
    }
    private void FixedUpdate()
    {
        base.FixedRefresh();
        if (bIsAlive)
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

    public void AttackMove()
    {
        if(fAttackWaitTimeCounter <= 0)
        {
            bCanFollow = false;
            // anim.applyRootMotion = true;
            anim.SetTrigger("StabAttack");
            StartCoroutine(ChangeBoolAfter((bool b) => {/*anim.applyRootMotion = b;*/ fAttackWaitTimeCounter = fAttackWaitTime; bCanFollow = true; bCanRotate = true; }, false, anim.GetCurrentAnimatorStateInfo(0).length));
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


}
