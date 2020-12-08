using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { SCORPION = 0 }
public class Enemy : MonoBehaviour, IHittable
{
    //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    const float fVISION_RANGE = 5f;
    const float fTARGET_FOLLOW_RANGE = 40f;
    const float fROTATE_SPEED = 200f;

    protected float fINVULNERABILITY_TIME = 0.5f;
    protected float fAttackRange = 5f;
    public float fMaxHitPoints;
    protected float fCurrentHitPoints;
    public float fSpeed;
    public int iDamage;
    protected Rigidbody rbody;
    protected Animator anim;
    protected PlayerController targetPlayer;

    protected bool bIsAlive;
    protected bool bCanMove;
    protected bool bCanRotate;
    protected bool bIsMoving = true;
    protected bool bIsInvulnerable;
    protected bool bIsHit;
    protected bool bTargetFound;
    protected bool bCanAttack;
    protected bool bCanFollow;
    public float fWalkTime;
    public float fWaitTime;
    public float fAttackWaitTime = 1.5f;
    protected float fAttackWaitTimeCounter;
    private Vector3 randomVector;
    private Vector3 startPosition;

    public void Initialize()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        targetPlayer = GameController.Instance.player;
        startPosition = transform.position;
        fAttackWaitTimeCounter = 0;
        bIsAlive = true;
    }
    public void Refresh()
    {

    }
    public void FixedRefresh()
    {

    }
    public void TakeDamage(int _damage)
    {
        if (!bIsInvulnerable)
        {
            IsInvulnerable(true);
            bIsHit = true;
            bTargetFound = true;
            fCurrentHitPoints -= _damage;

            StartCoroutine(ChangeBoolAfter((bool b) => { IsInvulnerable(b); bIsHit = b; }, false, fINVULNERABILITY_TIME));
        }
        if (fCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    private void OnCollisionEnter(Collision _collision)
    {
        if (bIsAlive)
        {
            {
                if (_collision.collider)
                {
                    if (_collision.collider.GetComponent<IHittable>() != null)
                    {
                        _collision.collider.GetComponent<IHittable>().TakeDamage(iDamage);
                    }
                }
            }
        }
    }
    public void Die()
    {
        bIsAlive = false;
        Destroy(gameObject, 4f);
    }
    public void MovingRandomly()
    {
        if (!bCanMove)
        {
            StartCoroutine(MoveRandom());
        }
        else
        {
            if (bIsMoving)
            {
                if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                {
                    bIsMoving = false;
                    StartCoroutine(ChangeBoolAfter((bool b) => { bCanMove = b; }, false, fWaitTime));
                }

                rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
            }
            else
            {
                if (bCanRotate)
                {
                    HelperFunctions.RotateTowardsTarget(transform, randomVector, fROTATE_SPEED);
                }
            }
        }
    }
    public void FindingTarget()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0), transform.forward * fVISION_RANGE, Color.red);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), transform.forward, out hit, fVISION_RANGE))
        {
            if (hit.transform.CompareTag("Player"))
            {
                bTargetFound = true;
                bCanFollow = true;
            }
        }
    }
    public void FollowTarget()
    {
        if (bCanFollow)
        {
            HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATE_SPEED);
            rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
        }
    }
    public void CheckTargetInRange()
    {
        fAttackWaitTimeCounter -= Time.deltaTime;
        // Out of Range
        if ((transform.position - targetPlayer.transform.position).sqrMagnitude >= fTARGET_FOLLOW_RANGE)
        {
            bCanAttack = false;
            bCanFollow = false;
            bTargetFound = false;
            bIsMoving = false;
            bCanRotate = false;
            bCanMove = true;
            StopAllCoroutines();
            StartCoroutine(ChangeBoolAfter((bool b) => { bIsMoving = b; bCanMove = !b; }, true, fWaitTime));
        }
        // In Attack Range
        else if ((transform.position - targetPlayer.transform.position).sqrMagnitude <= fAttackRange)
        {
            rbody.velocity = Vector3.zero;// HelperFunctions.VectorZero(rbody);
            if (fAttackWaitTimeCounter <= 0 && !bIsInvulnerable)
            {
                Vector3 dir = (targetPlayer.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(dir, transform.forward);
                if(dot > 0.95f)
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
        // In Non Attack Range
        else
        {
            bCanAttack = false;
        }
    }
    
    IEnumerator MoveRandom()
    {
        randomVector = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); // added transform position for rotating correctly

        if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        {
            transform.forward *= -1;// transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        }

        bIsMoving = true;
        bCanMove = true;
        yield return new WaitForSeconds(fWalkTime);
        bIsMoving = false;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanRotate = true;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanRotate = false;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanMove = false;
    }
    public IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }

    public void IsInvulnerable(bool _invulnerable)
    {
        bIsInvulnerable = _invulnerable;
    }
}
