using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { SCORPION = 0}
public class Enemy : MonoBehaviour, IHittable
{
   //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    const float fHIT_TIME = 0.7f;
    const float fVISION_RANGE = 5f;
    const float fTARGET_RANGE = 40f;

    public float fMaxHitPoints;
    protected float fCurrentHitPoints;
    public float fSpeed;
    public int iDamage;
    protected Rigidbody rbody;
    protected Animator anim;
    protected PlayerController targetPlayer;

    private bool bIsMoving;
    private bool bCanMove = true;
    private bool bIsHit;
    protected bool bTargetFound;
    public float fWalkTime;
    public float fWaitTime;
    private float fRotationSpeed = 80f;
    private Vector3 randomVector;
    private Vector3 startPosition;

    public void Initialize()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        targetPlayer = GameController.Instance.player;
        startPosition = transform.position;
    }

    public void Refresh()
    {
        CheckIfTargetOutOfRange();
        Debug.Log(bTargetFound);
    }

    public void TakeDamage(int _damage)
    {
        bIsHit = true;
        fCurrentHitPoints -= _damage;
        StopAllCoroutines();
        StartCoroutine(ChangeBoolAfter((bool b) => { bIsHit = b; }, false, fHIT_TIME));

        if (fCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider _collider)
    {
        if (!bIsHit)
        {
            if (_collider)
            {
                if (_collider.gameObject.GetComponent<ICanDamage>() != null)
                {
                    TakeDamage(_collider.gameObject.GetComponent<ICanDamage>().Damage());
                }
            }
        }
       
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void MovingRandomly()
    {
        FindingTarget();
        if (!bIsMoving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRandom());
        }
        else
        {
            if (bCanMove)
            {
                if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                {
                    bCanMove = false;
                    StartCoroutine(ChangeBoolAfter((bool b) => { bIsMoving = b; }, false, fWaitTime));
                }
                else
                {
                    if (randomVector != Vector3.zero)
                        transform.forward = new Vector3(randomVector.normalized.x, transform.forward.y, randomVector.normalized.z);
                    else
                        transform.forward = new Vector3(0.1f, transform.forward.y, 0.1f); // Fixed bug for look rotation (and speedy or no movement)

                    rbody.MovePosition(transform.position + (transform.forward * fSpeed * Time.fixedDeltaTime));
                }
            }
        }
    }
    public void FindingTarget()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0), transform.forward * fVISION_RANGE, Color.red);
        if(Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), transform.forward, out hit, fVISION_RANGE))
        {
            if (hit.transform.CompareTag("Player"))
            {
                bTargetFound = true;
            }
        }
    }
    public void FollowTarget()
    {
        HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fRotationSpeed);
        rbody.MovePosition(transform.position + (transform.forward * fSpeed * Time.fixedDeltaTime));
    }
    public void CheckIfTargetOutOfRange()
    {
        if (bTargetFound)
        {
            if((transform.position - targetPlayer.transform.position).sqrMagnitude >= fTARGET_RANGE)
            {
                bTargetFound = false;
                bIsMoving = true;
                bCanMove = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bCanMove = b; bIsMoving = !b; }, true, fWaitTime));
            }
        }
    }
    IEnumerator MoveRandom()
    {
        randomVector = new Vector3(Random.Range(1, -1), 0, Random.Range(-1, 1));

        if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        {
            randomVector *= -1; //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        }
        if (randomVector != Vector3.zero)
            transform.forward = new Vector3(randomVector.normalized.x, transform.forward.y, randomVector.normalized.z);
        //lastFacingDir = randomVector;
        bCanMove = true;
        bIsMoving = true;
        yield return new WaitForSeconds(fWalkTime);
        bCanMove = false;
        yield return new WaitForSeconds(fWaitTime);
        bIsMoving = false;
    }
    IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }
}
