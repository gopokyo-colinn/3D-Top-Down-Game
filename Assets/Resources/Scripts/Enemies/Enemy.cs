using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { METALON = 0, PLANT = 1 }
public class Enemy : MonoBehaviour
{
    float fInvulnerableCounter;

    //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    const float fVISION_RANGE = 5f;

    protected const float fROTATE_SPEED = 240f;
    protected float fAttackRange = 5f;
    protected float fFollowRange = 100f;

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
    protected bool bIsMoving;
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
    public float fOnCollisionKnockBackForce = 5f;
    public Vector3 tHeadOffset = new Vector3(0,0.5f,0);


    // Worst Obstacle Crossing Technique
    bool bObstacleInMyWay;
    bool bForwardHit;
    bool bLeftHit;
    bool bRightHit;
    Vector3 dirObstCross = Vector3.zero;
    float fObstacleCheckTimer = 0.3f;

    public void Initialize()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        fCurrentHitPoints = fMaxHitPoints;
        if (!targetPlayer)
            targetPlayer = PlayerController.Instance;
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
                        bTargetFound = true;
                        _collision.collider.GetComponent<IHittable>().ApplyKnockback(transform.position, fOnCollisionKnockBackForce);
                        _collision.collider.GetComponent<IHittable>().ApplyDamage(iCollisionDamage);
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
           StopAllCoroutines(); /// Bug: This could cause some bugs;
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
                    HelperFunctions.RotateTowardsTarget(transform, randomVector, Random.Range(80f, fROTATE_SPEED));
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
            MoveWithAIRaycastMethod();

            if (!bObstacleInMyWay)
            {
                HelperFunctions.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATE_SPEED);
            }
            else
                CheckObstaclesInPath();

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
    public void CalculateInvulnerability(float _fStunTime)
    {
        if (bIsInvulnerable)
        {
            fInvulnerableCounter += Time.deltaTime;
            if (fInvulnerableCounter >= anim.GetCurrentAnimatorStateInfo(0).length + _fStunTime)
            {
                bIsInvulnerable = false;
                bIsHit = false;
                bCanFollow = true;
                bCanAttack = false;
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
                //bCanMove = true;
                //// StopAllCoroutines();
                StartCoroutine(HelperFunctions.ChangeBoolAfter((bool b) => { bIsMoving = true; bCanMove = b; }, false, fWaitTime * 2f));
            }
        }
        // In Attack Range
        else if ((transform.position - targetPlayer.transform.position).sqrMagnitude <= _fAttackRange)
        {
            bCanFollow = false;
            //rbody.velocity = Vector3.zero;// HelperFunctions.VectorZero(rbody);
            if (!bIsInvulnerable)
            {
                if (fAttackWaitTimeCounter <= 0)
                {
                    Vector3 dir = (targetPlayer.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(dir, transform.forward); 
                    if (dot > 0.98f)  // if the enemy is facing the target
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
            bCanFollow = true;
            bCanAttack = false;
        }
    }
    public bool IsInvulnerable()
    {
        return bIsInvulnerable;
    }

    /// <summary> //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /Moving AIIII
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position + transform.forward * 2, 0.7f);
    }
    public bool CheckObstaclesInPath()
    {
        CheckRaysSideways(true);

        if (!bForwardHit && !bRightHit && !bLeftHit)
        {
            fObstacleCheckTimer -= Time.deltaTime;
            if (fObstacleCheckTimer <= 0)
            {
                bObstacleInMyWay = false; 
                dirObstCross = Vector3.zero;
                fObstacleCheckTimer = 0.4f;
            }
            return true;
        }
        else
        {
            fObstacleCheckTimer = 0.4f;
        }

        return false;
    }
    public void MoveWithAIRaycastMethod()
    {
        RaycastHit _hit;
        Ray _rForward = new Ray(transform.position + tHeadOffset, transform.forward);

        if (Physics.Raycast(_rForward, out _hit, 2f, LayerMask.GetMask("Player", "Ground", "Item", "Weapon"))) // obstacle is in way
        {
            bForwardHit = false;
        }
        else if (Physics.Raycast(_rForward, out _hit, 2f)) // obstacle is in way
        {
            bForwardHit = true; bObstacleInMyWay = true;

            if (dirObstCross == Vector3.zero)
            {
                dirObstCross = CheckRaysSideways();
            }
        }
        else
        {
            bForwardHit = false;
        }
        if (bObstacleInMyWay)
        {
            transform.forward = dirObstCross; // Rotating causes error
            //HelperFunctions.RotateTowardsTarget(transform, finalDir, fROTATE_SPEED); 
        }
       // Debug.DrawRay(transform.position + tHeadOffset, transform.forward * 2f);
    }
    public Vector3 CheckRaysSideways(bool _bRayCheckOnly = false)
    {
       // Debug.DrawRay(transform.position + tHeadOffset, (transform.forward + transform.right / 2) * 2f);
       // Debug.DrawRay(transform.position + tHeadOffset, (transform.forward - transform.right / 2) * 2f);
       
        Ray _rRight = new Ray(transform.position + tHeadOffset, (transform.forward + transform.right / 2));
        Ray _rLeft = new Ray(transform.position + tHeadOffset, (transform.forward - transform.right / 2));

        Vector3 _dirToMove = _rRight.direction;
        float _fClosestAngle = 9999f;

        if (!Physics.Raycast(_rRight, 2f))
        {
            bRightHit = false;
            if (!_bRayCheckOnly)
            {
                Vector3 targetDir = targetPlayer.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, _rRight.GetPoint(1f));

                if (angle < _fClosestAngle)
                {
                    _fClosestAngle = angle;
                }
                _dirToMove = _rRight.direction;
            }
        }
        else
        {
            bRightHit = true;
        }

        if (!Physics.Raycast(_rLeft, 2f))
        {
            bLeftHit = false;
            if (!_bRayCheckOnly)
            {
                Vector3 targetDir = targetPlayer.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, _rLeft.GetPoint(1f));

                if (angle < _fClosestAngle)
                {
                    _fClosestAngle = angle;
                    _dirToMove = _rLeft.direction;
                }
            }
        }
        else
        {
            bLeftHit = true; 
        }

        if (!_bRayCheckOnly)
        {
            if(bLeftHit && bRightHit)
            {
                _dirToMove = DrawRaysLeftRight();
            }
        }

        return _dirToMove;
    }
    public Vector3 DrawRaysLeftRight()
    {
        //Debug.DrawRay(_posssTP, (_posssRT) * 2f);
        //Debug.DrawRay(_posssTP, ( -_posssRT) * 2f);

        Ray _rTotalRight = new Ray(transform.position + tHeadOffset, (transform.right));
        Ray _rTotalLeft = new Ray(transform.position + tHeadOffset, (-transform.right));

        Vector3 _dirToMove = targetPlayer.transform.position;
        float _fClosestAngle = 9999f;

        if (!Physics.Raycast(_rTotalLeft, 2f))
        {
            Vector3 targetDir = targetPlayer.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, _rTotalLeft.GetPoint(1f));

            if (angle < _fClosestAngle)
            {
                _fClosestAngle = angle;
                _dirToMove = _rTotalLeft.direction;
            }
        }
        if (!Physics.Raycast(_rTotalRight, 2f))
        {
            Vector3 targetDir = targetPlayer.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, _rTotalRight.GetPoint(1f));

            if (angle < _fClosestAngle)
            {
                _fClosestAngle = angle;
                _dirToMove = _rTotalRight.direction;
            }
        }
        
        return _dirToMove;
    }

    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ApplyKnockback(Vector3 _sourcePosition, float _pushForce)
    {
        if (!bIsInvulnerable)
        {
            Vector3 pushForce = transform.position - _sourcePosition;
            pushForce.y = 0;
            //transform.forward = -pushForce.normalized;
            rbody.AddForce(pushForce.normalized * _pushForce - Physics.gravity * 0.2f, ForceMode.Impulse);
        }
    }
    public float GetCurrentHealth()
    {
        return fCurrentHitPoints;
    }
    public bool EnemyDied()
    {
        return !bIsAlive;
    }
}
