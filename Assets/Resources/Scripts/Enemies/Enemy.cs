using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { METALON = 0, PLANT = 1 }
public class Enemy : MonoBehaviour, IHittable
{
    float fInvulnerableCounter;

    //const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.6f;
    const float fVISION_RANGE = 5f;

    protected const float fROTATE_SPEED = 240f;
    protected float fAttackRange = 4f;
    protected float fFollowRange = 80f;

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
    protected bool bIsAttacking;
    protected bool bIsInvulnerable;
    protected bool bIsHit;
    protected bool bTargetFound;
    protected bool bCanAttack;
    protected bool bCanFollow;
    protected bool bIsGrounded;
    public float fWalkTime;
    public float fWaitTime;
    public float fAttackWaitTime = 1.5f;
    protected float fAttackWaitTimeCounter;
    private Vector3 randomVector;
    public float fOnCollisionKnockBackForce = 5f;
    public Vector3 tHeadOffset = new Vector3(0, 0.5f, 0);
    protected Vector3 moveVector;
    // Patrolling
    // Patrolling
    protected bool bIsPatroller;
    private Vector3 lastFacingDirection;
    private bool bDirReversing;
    protected bool bReverseDirection;
    private int iPatrolPos = 0;
    // Material Dissolve Variables
    private Material rndMaterial;
    float fMatDissolveAlpha = -0.8f;
    // Astar Pathfinding Variables
    //   bool bPathSuccess;
    //  protected bool bFollowingPath;
    //  int iPathIndex = 0;
    //   Vector3[] pathToFollow = new Vector3[0];
    /// Walk Area Variables
    public float fMaxWalkingDistance = 80;
    private Vector3 startPosition;
    Collider maxTravelAreaCol;

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

        // Just Randomiaing the stats a bit
        fSpeed = Random.Range(fSpeed - .5f, fSpeed + 1f);
        if (fWaitTime > 2)
            fWaitTime = Random.Range(fWaitTime - 1f, fWaitTime + 1f);
        if (fWalkTime > 2)
            fWaitTime = Random.Range(fWalkTime - 1f, fWalkTime + 1f);

        rndMaterial = GetComponentInChildren<Renderer>().material;

        StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsMoving = b; }, true, fWaitTime));
    }
    public void Refresh()
    {
        bIsGrounded = Grounded(transform, 0.4f);
        if (!bIsAlive)
        {
            DissolveOnDeath(0.6f);
        }
    }
    public void FixedRefresh()
    {
        if (bIsAlive)
        {
            if (bIsGrounded)
            { 
                CheckWalkingArea(startPosition);
            }
        }
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Movements and Target Checks
    public bool Grounded(Transform _transform, float _distanceToGround)
    {
        return Physics.Raycast(_transform.position + new Vector3(0, 0.2f, 0), Vector3.down, _distanceToGround);
    }
    public void FindingTarget()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, 0.3f, 0), transform.forward * fVISION_RANGE, Color.red);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), transform.forward, out hit, fVISION_RANGE))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                bTargetFound = true;
                bCanFollow = true;
                //bFollowingPath = false;
            }
        }
    }
    public void MoveRandomly()
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
                //if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                //{
                //    bIsMoving = false;
                //    StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bCanMove = b; }, false, fWaitTime));
                //}
                moveVector = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
                rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
               // rbody.velocity = moveVector * fSpeed * Time.fixedDeltaTime;
            }
            else
            {
                if (bCanRotate)
                {
                    HelpUtils.RotateTowardsTarget(transform, randomVector, fROTATE_SPEED);
                }
            }
        }
    }
    public void DoPatrolling(Transform[] _patrollingPoints)
    {
        Debug.Log(iPatrolPos);
        if (bIsMoving)
        {
            lastFacingDirection = (_patrollingPoints[iPatrolPos].position - transform.position).normalized;
            lastFacingDirection.y = 0;

            if ((transform.position - _patrollingPoints[iPatrolPos].position).sqrMagnitude <= 1f)
            {
                bIsMoving = false;
                StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsMoving = b; }, true, fWaitTime));

                if (bDirReversing)
                    iPatrolPos--;
                else
                    iPatrolPos++;

                if (iPatrolPos >= _patrollingPoints.Length)
                {
                    if (bReverseDirection)
                    {
                        iPatrolPos = _patrollingPoints.Length - 2;
                        bDirReversing = true;
                    }
                    else
                        iPatrolPos = 0;
                }
                else if (iPatrolPos == 0)
                {
                    if (bDirReversing)
                    {
                        bDirReversing = false;
                    }
                }
            }
           
            transform.forward = lastFacingDirection;
            moveVector = new Vector3(lastFacingDirection.x, 0, lastFacingDirection.z);
            rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
            //rbody.velocity = moveVector * fSpeed * Time.fixedDeltaTime;
        }
        else
        {
            HelpUtils.RotateTowardsTarget(transform, _patrollingPoints[iPatrolPos].position, fROTATE_SPEED);
        }
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
                ResetBools();
                //bIsMoving = true;
                //bCanMove = false;
                //bTargetFound = false;
                // StopAllCoroutines();
                //StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsMoving = true; bCanMove = b; }, false, fWaitTime * 2f));
            }
        }
        // In Attack Range
        else if ((transform.position - targetPlayer.transform.position).sqrMagnitude <= _fAttackRange)
        {
            bCanFollow = false;
            bIsMoving = false;
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
                        HelpUtils.RotateTowardsTarget(transform, targetPlayer.transform.position, fROTATE_SPEED / 3f);
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
    IEnumerator SetRandomDirection()
    {
        randomVector = transform.position + Random.insideUnitSphere * 100f;
        randomVector.y = 0;
        
        if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        {
            randomVector = transform.position + Random.insideUnitSphere * 100f;// transform.forward *= -1;// transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
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
    public void FollowTarget(Vector3 _targetPosition)
    {
        if (!bIsAttacking && !bIsInvulnerable)
        {
            bIsMoving = true;
            HelpUtils.RotateTowardsTarget(transform, _targetPosition, fROTATE_SPEED);
            moveVector = new Vector3(transform.forward.x, 0, transform.forward.z);
            rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
            //rbody.velocity = moveVector * fSpeed * Time.fixedDeltaTime;
        }
    }
    public void CheckWalkingArea(Vector3 _targetPosition) // Walk Area Check 
    {
        if (!bTargetFound)
        {
            if (!bIsPatroller)
            {
                if ((transform.position - _targetPosition).sqrMagnitude > fMaxWalkingDistance)
                {
                    ResetBools();  
                    bIsMoving = true;
                    Vector3 _targetPos = (_targetPosition - transform.position).normalized;
                    transform.forward = _targetPos;
                    moveVector = new Vector3(_targetPos.x, 0, _targetPos.z);
                    rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
                    //rbody.velocity = moveVector * fSpeed * Time.fixedDeltaTime;
                }
            }
        }
        if (maxTravelAreaCol)
        {
            if (!maxTravelAreaCol.bounds.Contains(transform.position))
            {
                ResetBools();
                bIsMoving = true;
                Vector3 _targetPos = (_targetPosition - transform.position).normalized;
                transform.forward = _targetPos;
                moveVector = new Vector3(_targetPos.x, 0, _targetPos.z);
                rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Damages and Health Effects
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
    public void ApplyDamage(float _damage)
    {
        if (!bIsInvulnerable)
        {
            // Knockback(targetPlayer.transform.position, fPUSHBACKFORCE);
            bIsInvulnerable = true;
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
    public float GetCurrentHealth()
    {
        return fCurrentHitPoints;
    }
    public void Die()
    {
        bIsAlive = false;
        StartCoroutine(HelpUtils.WaitForSeconds(OnDeathStuff, 1f));
        //Destroy(gameObject, 4f);
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject, 5f);
    }
    public void Revive(Vector3 _position)
    {
        bIsAlive = true;
        fCurrentHitPoints = fMaxHitPoints;
        transform.position = _position;
    }
    void OnDeathStuff()
    {
        rbody.isKinematic = true;
        Collider[] _colliders = GetComponents<Collider>();
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
        StartCoroutine(HelpUtils.WaitForSeconds(delegate { gameObject.SetActive(false); }, 4f));
    }// MAKING IT KINEMETIC ON DEATH, AND REMOVING COLLIDERS
    void DissolveOnDeath(float _fDissolveSpeed)
    {
        fMatDissolveAlpha += _fDissolveSpeed * Time.deltaTime;
        rndMaterial.SetFloat("_alpha", fMatDissolveAlpha);
    }
    public bool IsInvulnerable()
    {
        return bIsInvulnerable;
    }
    public bool IsEnemyDead()
    {
        return !bIsAlive;
    }
    public void SetBoundary(Collider _boundaryCol)
    {
        maxTravelAreaCol = _boundaryCol;
    }
    public bool CanFollow()
    {
        return bCanFollow;
    }
    public void ResetWalkingBool()
    {
        bCanMove = false;
    }
    public void ResetBools()
    {
        StopAllCoroutines();
        bCanAttack = false;
        bCanFollow = false;
        bTargetFound = false;
        bIsMoving = false;
        bCanRotate = false;
        bCanMove = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Obstacle Checking
    //public bool IsObstacleBetweenTargetAndMe(Vector3 _targetPosition)
    //{
    //    Ray _rayToPlayer = new Ray(transform.position + tHeadOffset, (_targetPosition - transform.position).normalized);
    //    RaycastHit _hit;
    //    Debug.DrawRay(transform.position, (_targetPosition - transform.position).normalized * 10f);

    //    if (Physics.Raycast(_rayToPlayer, out _hit, 10f))
    //    {
    //        if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
    //            return false;
    //    }
    //    return true;
    //}
    //public bool IsObstaclesInFront()
    //{
    //    RaycastHit _hit;

    //    if (Physics.BoxCast(transform.position + tHeadOffset, transform.localScale / 2, transform.forward, out _hit, Quaternion.identity, 2f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)) // obstacle is in way
    //    {
    //        //Debug.Log(_hit.collider.gameObject.name);
    //        return true;
    //    }
    //    return false;
    //}

    /// AStar Function for Walk Area and Target Follow ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //public void FollowTargetAStar(Vector3 _targetPosition) // Right now Follow Target With AStar Algorithm (Gets too laggy sometimes)
    //{
    //    if (!bIsAttacking && !bIsInvulnerable)
    //    {
    //        //            Debug.Log(bFollowingPath);
    //        if (IsObstacleBetweenTargetAndMe(_targetPosition)) // obstacle is in way
    //        {
    //            // bFollowingPath = false;
    //            RequestPath(_targetPosition);
    //        }
    //        else
    //        {
    //            HelpUtils.RotateTowardsTarget(transform, _targetPosition, fROTATE_SPEED);
    //            rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
    //        }
    //        //else
    //        if (bFollowingPath)
    //        {
    //            FollowPath();
    //        }
    //    }
    //}
    //public void CheckWalkingAreaAStar(Vector3 _targetPosition)
    //{
    //    if ((transform.position - startPosition).sqrMagnitude > fMaxWalkingDistance)
    //    {
    //        RequestPath(_targetPosition);
    //    }
    //    if (bFollowingPath && !bTargetFound)
    //    {
    //        FollowPath();
    //    }
    //}// Walk Area Check with AStar Algorithm (Too laggy with multiple units)

    /// Pathfinding Requests
    //public void RequestPath(Vector3 _targetPos)
    //{
    //    if (!bFollowingPath)
    //    {
    //        Debug.Log("Path Requested Complete");
    //        bFollowingPath = true;
    //        PathFindingManager.RequestPath(new PathRequest(transform.position, _targetPos, OnPathRequest));
    //        iPathIndex = 0;
    //    }

    //}
    //public void OnPathRequest(Vector3[] _path, bool _bSuccess)
    //{
    //    if (_bSuccess)
    //    {
    //        pathToFollow = _path;
    //    }
    //}
    //public void FollowPath()
    //{
    //    if (pathToFollow != null)
    //    {
    //        if(pathToFollow.Length > 0)
    //        {
    //            if(iPathIndex < pathToFollow.Length)
    //            {
    //                Vector3 _nextPosition = new Vector3(pathToFollow[iPathIndex].x, transform.position.y, pathToFollow[iPathIndex].z);

    //               // transform.forward = (pathToFollow[iPathIndex] - transform.position).normalized;
    //                HelpUtils.RotateTowardsTarget(transform, pathToFollow[iPathIndex], fROTATE_SPEED / 2);

    //                if ((_nextPosition - transform.position).sqrMagnitude < 1)
    //                {
    //                    iPathIndex++;
    //                }
    //            }

    //            if ((transform.position - new Vector3(pathToFollow[pathToFollow.Length - 1].x, transform.position.y, pathToFollow[pathToFollow.Length - 1].z)).sqrMagnitude < 4)//iPathIndex >= pathToFollow.Length)
    //            {
    //                bFollowingPath = false;
    //            }
    //        }
    //    }
    //    rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
    //}
}