using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { SCORPION = 0 }
public class Enemy : MonoBehaviour, IHittable
{
    //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    const float fHIT_TIME = 0.7f;
    const float fVISION_RANGE = 5f;
    const float fTARGET_RANGE = 40f;
    const float fROTATESPEED = 80f;

    public float fMaxHitPoints;
    protected float fCurrentHitPoints;
    public float fSpeed;
    public int iDamage;
    protected Rigidbody rbody;
    protected Animator anim;
    protected PlayerController targetPlayer;

    private bool bCanMove;
    private bool bCanRotate;
    private bool bIsMoving = true;
    private bool bIsHit;
    protected bool bTargetFound;
    public float fWalkTime;
    public float fWaitTime;
    private Vector3 randomVector;
    private Vector3 startPosition;

    public GameObject tt;
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
    }
    public void FixedRefresh()
    {

    }

    public void TakeDamage(int _damage)
    {
        bIsHit = true;
        fCurrentHitPoints -= _damage;
       // StopAllCoroutines();
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

                rbody.MovePosition(transform.position + (transform.forward * fSpeed * Time.fixedDeltaTime));
            }
            else
            {
                if (bCanRotate)
                {
                    HelperFunctions.RotateTowardsTarget(transform, randomVector, fROTATESPEED);
                }
            }
        }
    }
    public void FindingTarget()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0), transform.forward * fVISION_RANGE, Color.red);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), transform.forward, out hit, fVISION_RANGE))
        {
            if (hit.transform.CompareTag("Player"))
            {
                bTargetFound = true;
            }
        }
    }
    public void FollowTarget()
    {
        HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATESPEED);
        rbody.MovePosition(transform.position + (transform.forward * fSpeed * Time.fixedDeltaTime));
    }
    public void CheckIfTargetOutOfRange()
    {
        if (bTargetFound)
        {
            if ((transform.position - targetPlayer.transform.position).sqrMagnitude >= fTARGET_RANGE)
            {
                bTargetFound = false;
                bCanMove = true;
                bIsMoving = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bIsMoving = b; bCanMove = !b; }, true, fWaitTime));
            }
        }
    }
    IEnumerator MoveRandom()
    {
        randomVector = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); // added transform position for rotating correctly
        tt.transform.position = randomVector;

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
    IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }
}
