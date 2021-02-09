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
    protected float fFollowRange = 120f;

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
    public float fWalkTime;
    public float fWaitTime;
    public float fAttackWaitTime = 1.5f;
    protected float fAttackWaitTimeCounter;
    private Vector3 randomVector;
    public float fOnCollisionKnockBackForce = 5f;
    public Vector3 tHeadOffset = new Vector3(0, 0.5f, 0);

    // Astar Pathfinding Variables
    bool bPathSuccess;
    protected bool bFollowingPath;
    int iPathIndex = 0;
    Vector3[] pathToFollow = new Vector3[0];

    //// Worst Obstacle Crossing Technique
    //bool bObstacleInMyWay;
    //bool bForwardHit;
    //bool bLeftHit;
    //bool bRightHit;
    //bool bTotalRightHit;
    //bool bTotalLeftHit;
    //Vector3 dirToObstacleCross = Vector3.zero;
    //float fObstacleCheckTimer = 0.3f;

    // Walk Area Variables
    public float fMaxWalkingDistance = 40;
    private Vector3 startPosition;

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
        if (bIsAlive)
        {
            if (!bTargetFound) 
            {
               //CheckWalkingAreaAStar(startPosition); 
            }
        }
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Movements and Target Checks
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
                if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                {
                    bIsMoving = false;
                    StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bCanMove = b; }, false, fWaitTime));
                }

                rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
            }
            else
            {
                if (bCanRotate)
                {
                    HelpUtils.RotateTowardsTarget(transform, randomVector, Random.Range(80f, fROTATE_SPEED));
                }
            }
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
                bCanAttack = false;
                bCanFollow = false;
                bTargetFound = false;
                bIsMoving = false;
                bCanRotate = false;
                //bCanMove = true;
                //// StopAllCoroutines();
                StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsMoving = true; bCanMove = b; }, false, fWaitTime * 2f));
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
        randomVector = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1f, -1f)); // added transform position for rotating correctly
        if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
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
    public void FollowTarget(Vector3 _targetPosition)
    {
        if (!bIsAttacking && !bIsInvulnerable)
        {
            HelpUtils.RotateTowardsTarget(transform, _targetPosition, fROTATE_SPEED);
            rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Obstacle Checking
    public bool IsObstacleBetweenTargetAndMe(Vector3 _targetPosition)
    {
        Ray _rayToPlayer = new Ray(transform.position + tHeadOffset, (_targetPosition - transform.position).normalized);
        RaycastHit _hit;
        Debug.DrawRay(transform.position, (_targetPosition - transform.position).normalized * 10f);

        if (Physics.Raycast(_rayToPlayer, out _hit, 10f))
        {
            if(_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                return false;
        }
        return true;
    }
    public bool IsObstaclesInFront()
    {
        RaycastHit _hit;

        if (Physics.BoxCast(transform.position  + tHeadOffset, transform.localScale / 2, transform.forward, out _hit, Quaternion.identity, 2f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)) // obstacle is in way
        {
            //Debug.Log(_hit.collider.gameObject.name);
            return true;
        }
        return false;
    }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Obstacle Crossing Super Fuddu Bakwas AI
  /*  public bool CheckObstaclesInPath(Vector3 _targetPositin)
    {
        CheckRaysSideways(_targetPositin, true);
        DrawRaysLeftRight(_targetPositin, dirToObstacleCross, 9999, true);

        if (!bForwardHit && !bRightHit && !bLeftHit && !bTotalLeftHit && !bTotalRightHit)
        {
            fObstacleCheckTimer -= Time.deltaTime;
            if (fObstacleCheckTimer <= 0)
            {
                bObstacleInMyWay = false;
                dirToObstacleCross = Vector3.zero;
                fObstacleCheckTimer = 0.4f;
            }
            return true;
        }
        else
        {
            // dirToObstacleCross = Vector3.zero;
            fObstacleCheckTimer = 0.4f;
        }

        return false;
    }
    public void MoveWithAIRaycastMethod(Vector3 _targetPositin)
    {
        RaycastHit _hit;
        Ray _rForward = new Ray(transform.position + tHeadOffset, transform.forward);

        if (Physics.BoxCast(transform.position + tHeadOffset, transform.localScale * 2, transform.forward, out _hit, Quaternion.identity, 3f, LayerMask.GetMask("Player", "Ground", "Item", "Weapon"))) // obstacle is in way
        {
            bForwardHit = false;
        }
        else if (Physics.BoxCast(transform.position + tHeadOffset, transform.localScale * 2, transform.forward, out _hit, Quaternion.identity, 3f)) // obstacle is in way
        {
            bForwardHit = true;
            bObstacleInMyWay = true;

            if (dirToObstacleCross == Vector3.zero)
            {
                dirToObstacleCross = CheckRaysSideways(_targetPositin);
            }
        }
        else
        {
            bForwardHit = false;
        }
        if (bObstacleInMyWay)
        {
            if (bForwardHit)
                transform.forward = dirToObstacleCross; // Rotating causes error
                                                        //HelpUtils.RotateTowardsTarget(transform, dirToObstacleCross, fROTATE_SPEED); 
        }
        // Debug.DrawRay(transform.position + tHeadOffset, transform.forward * 2f);
    }
    public Vector3 CheckRaysSideways(Vector3 _targetPositin, bool _bRayCheckOnly = false)
    {
        Debug.DrawRay(transform.position + tHeadOffset, (transform.forward + transform.right / 2) * 2f);
        Debug.DrawRay(transform.position + tHeadOffset, (transform.forward - transform.right / 2) * 2f);

        Ray _rRight = new Ray(transform.position + tHeadOffset, (transform.forward + transform.right / 2));
        Ray _rLeft = new Ray(transform.position + tHeadOffset, (transform.forward - transform.right / 2));

        Vector3 _dirToMove = _rRight.direction;
        float _fClosestAngle = 9999f;

        if (!Physics.Raycast(_rRight, 2f))
        {
            bRightHit = false;
            if (!_bRayCheckOnly)
            {
                Vector3 targetDir = _targetPositin - transform.position;
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
                Vector3 targetDir = _targetPositin - transform.position;
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
            // if (bLeftHit && bRightHit)
            {
                _dirToMove = DrawRaysLeftRight(_targetPositin, _dirToMove, _fClosestAngle);
            }
        }

        return _dirToMove;
    }
    public Vector3 DrawRaysLeftRight(Vector3 _targetPositin, Vector3 _defaultDirection, float _fClosestAngle = 9999f, bool _bCheckOnly = false)
    {
        Ray _rTotalRight = new Ray(transform.position + tHeadOffset, (transform.right));
        Ray _rTotalLeft = new Ray(transform.position + tHeadOffset, (-transform.right));

        Vector3 _dirToMove = _targetPositin;
        // float _fClosestAngle = 9999f;

        if (!Physics.Raycast(_rTotalLeft, 2f))
        {
            bTotalLeftHit = false;
            if (!_bCheckOnly)
            {
                Vector3 targetDir = _targetPositin - transform.position;
                float angle = Vector3.Angle(targetDir, _rTotalLeft.GetPoint(1f));

                if (angle < _fClosestAngle)
                    //            {
                    _fClosestAngle = angle;
                _dirToMove = _rTotalLeft.direction;
            }
            else
                _dirToMove = _defaultDirection;
        }
    }
        else
        {
            bTotalLeftHit = true;
        }
        if (!Physics.Raycast(_rTotalRight, 2f))
        {
            bTotalRightHit = false;
            if (!_bCheckOnly)
            {
                Vector3 targetDir = _targetPositin - transform.position;
float angle = Vector3.Angle(targetDir, _rTotalRight.GetPoint(1f));

                if (angle<_fClosestAngle)
                {
                    _fClosestAngle = angle;
                    _dirToMove = _rTotalRight.direction;
                }
                else
                    _dirToMove = _defaultDirection;
            }
        }
        else
        {
            bTotalRightHit = true;
        }

        return _dirToMove;
    }
    public void FollowTargetWithNoobAI(Vector3 _targetPos)
{
    if (!bIsAttacking)
    {
        MoveWithAIRaycastMethod(_targetPos);

        if (!bObstacleInMyWay)
        {
            HelpUtils.RotateTowardsTarget(transform, _targetPos, fROTATE_SPEED);
        }
        else
            CheckObstaclesInPath(_targetPos);

        rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
    }
}*/

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
    public float GetCurrentHealth()
    {
        return fCurrentHitPoints;
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
    public bool IsInvulnerable()
    {
        return bIsInvulnerable;
    }
    public bool IsEnemyDead()
    {
        return !bIsAlive;
    }

    //// AStar Function for Walk Area and Target Follow ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void FollowTargetAStar(Vector3 _targetPosition) // Right now Follow Target With AStar Algorithm (Gets too laggy sometimes)
    {
        if (!bIsAttacking && !bIsInvulnerable)
        {
            //            Debug.Log(bFollowingPath);
            if (IsObstacleBetweenTargetAndMe(_targetPosition)) // obstacle is in way
            {
                // bFollowingPath = false;
                RequestPath(_targetPosition);
            }
            else
            {
                HelpUtils.RotateTowardsTarget(transform, _targetPosition, fROTATE_SPEED);
                rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
            }
            //else
            if (bFollowingPath)
            {
                FollowPath();
            }
        }
    }
    public void CheckWalkingAreaAStar(Vector3 _targetPosition)
    {
        if ((transform.position - startPosition).sqrMagnitude > fMaxWalkingDistance)
        {
            RequestPath(_targetPosition);
        }
        if (bFollowingPath && !bTargetFound)
        {
            FollowPath();
        }
    }// Walk Area Check with AStar Algorithm (Too laggy with multiple units)

    /// Pathfinding Requests
    public void RequestPath(Vector3 _targetPos)
    {
        if (!bFollowingPath)
        {
            Debug.Log("Path Requested Complete");
            bFollowingPath = true;
            PathFindingManager.RequestPath(new PathRequest(transform.position, _targetPos, OnPathRequest));
            iPathIndex = 0;
        }

    }
    public void OnPathRequest(Vector3[] _path, bool _bSuccess)
    {
        if (_bSuccess)
        {
            pathToFollow = _path;
        }
    }
    public void FollowPath()
    {
        if (pathToFollow != null)
        {
            if(pathToFollow.Length > 0)
            {
                if(iPathIndex < pathToFollow.Length)
                {
                    Vector3 _nextPosition = new Vector3(pathToFollow[iPathIndex].x, transform.position.y, pathToFollow[iPathIndex].z);

                   // transform.forward = (pathToFollow[iPathIndex] - transform.position).normalized;
                    HelpUtils.RotateTowardsTarget(transform, pathToFollow[iPathIndex], fROTATE_SPEED / 2);

                    if ((_nextPosition - transform.position).sqrMagnitude < 1)
                    {
                        iPathIndex++;
                    }
                }
           
                if ((transform.position - new Vector3(pathToFollow[pathToFollow.Length - 1].x, transform.position.y, pathToFollow[pathToFollow.Length - 1].z)).sqrMagnitude < 4)//iPathIndex >= pathToFollow.Length)
                {
                    bFollowingPath = false;
                }
            }
        }
        rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
    }
}