using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { SCORPION = 0 }
public class Enemy : MonoBehaviour
{
    float fInvulnerableCounter;

    //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    const float fVISION_RANGE = 5f;
    protected const float fROTATE_SPEED = 140f;

    protected float fAttackRange = 5f;
    public float fMaxHitPoints;
    protected float fCurrentHitPoints;
    public float fSpeed;
    public int iCollisionDamage;
    protected Rigidbody rbody;
    protected Animator anim;
    protected static PlayerController targetPlayer;

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
        if (!targetPlayer)
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
    private void OnCollisionEnter(Collision _collision)
    {
        if (bIsAlive)
        {
            if (_collision.collider)
            {
                if (_collision.collider.gameObject.CompareTag("Player"))
                {
                    if (_collision.collider.GetComponent<IHittable>() != null)
                    {
                        _collision.collider.GetComponent<IHittable>().TakeDamage(iCollisionDamage);
                    }
                }
            }
        }
    }
    public void Die()
    {
        bIsAlive = false;
        rbody.isKinematic = true;
        Destroy(gameObject, 4f);
        Collider[] _colliders = GetComponents<Collider>();
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
    }
    public void MovingRandomly()
    {
        if (!bCanMove)
        {
           StartCoroutine(SetRandomDirection());
        }
        else
        {
            if (bIsMoving)
            {
                if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                {
                    bIsMoving = false;
                    StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { bCanMove = b; }, false, fWaitTime));
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
        if (bCanFollow && !bIsInvulnerable)
        {
            HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATE_SPEED);
            rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
        }
    }
    IEnumerator SetRandomDirection()
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
    public void CalculateInvulnerability(float _fInvulnerableTime)
    {
        if (bIsInvulnerable)
        {
            fInvulnerableCounter += Time.deltaTime;
            if (fInvulnerableCounter >= anim.GetCurrentAnimatorStateInfo(0).length + _fInvulnerableTime)
            {
                IsInvulnerable(false);
                bIsHit = false;
                bCanFollow = true;
                fInvulnerableCounter = 0;
            }
        }
    }
    public void CheckTargetInRange(float _fAttackRange, float _fTargetFollowRange)
    {
        fAttackWaitTimeCounter -= Time.deltaTime;
        // Out of Range
        if ((transform.position - targetPlayer.transform.position).sqrMagnitude >= _fTargetFollowRange)
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
        else if ((transform.position - targetPlayer.transform.position).sqrMagnitude <= _fAttackRange)
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
    public void IsInvulnerable(bool _invulnerable)
    {
        bIsInvulnerable = _invulnerable;
    }
    /* public void SetRandomDirection()
     {

         randomVector = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); // added transform position for rotating correctly
         if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
         {
             transform.forward *= -1;// transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
         }
         bIsMoving = true;
         bCanMove = true;

     }
     public void RandomMovementCounter()
     {
         fRandomWaitCounter += Time.deltaTime;
         fRandomWalkCounter += Time.deltaTime;

         if (bIsMoving && fRandomWalkCounter >= fWalkTime)
         {
             bIsMoving = false;
             fRandomWalkCounter = 0;
         }

         if (!bIsMoving)
         {
             if (fRandomWaitCounter >= fWaitTime / 3 && !bCanRotate)
                 bCanRotate = true;
             else if (fRandomWaitCounter >= 2 * (fWaitTime / 3) && bCanRotate)
                 bCanRotate = false;
             else if (fRandomWaitCounter >= 3 * (fWaitTime / 3) && bCanMove)
             {
                 bCanMove = false;
                 fRandomWaitCounter = 0;
             }
         }

     }*/

    public void Knockback(Vector3 _sourcePosition, float _pushForce)
    {
        Vector3 pushForce = transform.position - _sourcePosition;
        pushForce.y = 0;
        //transform.forward = -pushForce.normalized;
        rbody.AddForce(pushForce.normalized * _pushForce - Physics.gravity * 0.2f, ForceMode.Impulse);
    }
}
