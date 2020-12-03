using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { SCORPION = 0}
public class Enemy : MonoBehaviour, IHittable
{
    const float DISTANCE_TO_GROUND = 0.1f;
    const float DISTANCE_TO_COLIS = 1.2f;
    const float HIT_TIME = 0.5f;

    public float fMaxHitPoints;
    protected float fCurrentHitPoints;
    public float fSpeed;
    public int iDamage;
    protected Rigidbody rbody;
    protected Animator anim;
    protected PlayerController player;

    private bool isMovingRandomly;
    private bool canMove = true;
    private bool bIsHit;
    public float fWalkTime;
    public float fWaitTime;
    private Vector3 randomVector;

    public void Initialize()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        player = GameController.Instance.player;
    }

    public void Refresh()
    {
        Debug.Log(bIsHit);
    }

    public void TakeDamage(int _damage)
    {
        if (!bIsHit)
        {
            bIsHit = true;
            fCurrentHitPoints -= _damage;
            Debug.Log(fCurrentHitPoints);
            StopAllCoroutines();
            StartCoroutine(ChangeBoolAfter((bool b) => { bIsHit = b; }, false, HIT_TIME));
        }
       
        if (fCurrentHitPoints <= 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider)
        {
           if (_collider.gameObject.GetComponent<ICanDamage>() != null)
           {
               TakeDamage(_collider.gameObject.GetComponent<ICanDamage>().Damage());
           }
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void MovingRandomly()
    {
        if (!isMovingRandomly)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRandom());
        }
        else
        {
            if (canMove)
            {
                if (HelperFunctions.CheckAheadForColi(transform, DISTANCE_TO_COLIS))
                {
                    canMove = false;
                    StartCoroutine(ChangeBoolAfter((bool b) => { isMovingRandomly = b; }, false, fWaitTime));
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
    IEnumerator MoveRandom()
    {
        randomVector = new Vector3(Random.Range(1, -1), 0, Random.Range(-1, 1));

        if (HelperFunctions.CheckAheadForColi(transform, DISTANCE_TO_COLIS))
        {
            randomVector *= -1; //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        }
        if (randomVector != Vector3.zero)
            transform.forward = new Vector3(randomVector.normalized.x, transform.forward.y, randomVector.normalized.z);
        //lastFacingDir = randomVector;
        canMove = true;
        isMovingRandomly = true;
        yield return new WaitForSeconds(fWalkTime);
        canMove = false;
        yield return new WaitForSeconds(fWaitTime);
        isMovingRandomly = false;
    }
    IEnumerator ChangeBoolAfter(System.Action<bool> _callBack, bool _setBool, float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        //StopAllCoroutines();
    }
}
