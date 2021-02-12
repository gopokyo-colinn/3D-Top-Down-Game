using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    Rigidbody rb;
    public float fSpeed = 10;
    List<GameObject> objectsInCollisionRangeLst; // if it takes more performance then make it just one single object to check for.
    bool bIsHit;
    public float fAvoidanceThreshold = 2f;
    public float fDetectionRadius = 1f;
    public bool bCanAvoidLargeScaleObstacles;
    Vector3 direction;
    Vector3 steerVelocity = Vector3.zero;
    Vector3 tHeadOffset = new Vector3(0,.5f,0);
    public bool bMakeThemWalkable;
    bool bIsEnemy;
    Collider[] _colliders;
    // Start is called before the first frame update
    void Start()
    {
        objectsInCollisionRangeLst = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        if(bMakeThemWalkable)
            fSpeed = Random.Range(90, 180);
        bIsEnemy = GetComponent<Enemy>() ? true : false;
       // player = PlayerController.Instance;
    }
    private void Update()
    {
        bIsHit = GetObjectHit();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (bMakeThemWalkable)
        {
            rb.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
        }
        if (bIsHit)
        {
           if (objectsInCollisionRangeLst != null)
           {
                foreach (var _go in objectsInCollisionRangeLst)
                {
                    CollisionAvoidance(_go.transform.position);
                }
           }
        }
    }

    bool GetObjectHit()
    {
        if(bIsEnemy) // if its enemy then it should not avoid collision with player
            _colliders = Physics.OverlapCapsule(transform.position + tHeadOffset + transform.forward / 2, transform.position + tHeadOffset + transform.forward * fAvoidanceThreshold /* detectionLength */, fDetectionRadius, LayerMask.GetMask("Default", "Enemy", "Npc"), QueryTriggerInteraction.Ignore);
            //_colliders = Physics.OverlapSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionSize, LayerMask.GetMask("Default", "Enemy", "Npc"));
        else
            _colliders = Physics.OverlapCapsule(transform.position + tHeadOffset + transform.forward / 2, transform.position + tHeadOffset + transform.forward * fAvoidanceThreshold, fDetectionRadius, LayerMask.GetMask("Default", "Player", "Enemy", "Npc"), QueryTriggerInteraction.Ignore);
        //_colliders = Physics.OverlapSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionSize, LayerMask.GetMask("Default", "Player", "Enemy", "Npc"));

        foreach (var _coli in _colliders)
        {
            if (_coli.isTrigger || _coli.gameObject == gameObject)
            {
                continue;
            }
            else
            {
                if(!objectsInCollisionRangeLst.Contains(_coli.gameObject))
                    objectsInCollisionRangeLst.Add(_coli.gameObject);
                return true;
            }
        }
        objectsInCollisionRangeLst.Clear();
        return false;
    }
    void CollisionAvoidance(Vector3 _targetPosition)
    {
        Vector3 distanceVector = _targetPosition - transform.position;
        float fLength = distanceVector.sqrMagnitude;
        
        if (fLength < fAvoidanceThreshold)
        {
            Vector3 adjVector = distanceVector.normalized;
            adjVector.y = 0;
            float difference = fAvoidanceThreshold - fLength;
            adjVector = adjVector * difference;
            steerVelocity = (rb.velocity - adjVector).normalized;
            steerVelocity.y = 0;

            //if (bCanAvoidLargeScaleObstacles)
            //{
            //    float xVel = transform.InverseTransformDirection(newVelocity).x;

            //    if (xVel > 0)
            //        rb.velocity = newVelocity + transform.right * (fSpeed / 1.2f) * Time.fixedDeltaTime;
            //    else
            //        rb.velocity = newVelocity - transform.right * (fSpeed / 1.2f)  * Time.fixedDeltaTime;
            //}

            // TODO: Do Something to make the character look in the direction of facing the steerVelocity

            rb.velocity = steerVelocity * fSpeed * Time.fixedDeltaTime; // normal avoidance, it even stop object if there is no forward motion            

            if (!bIsEnemy && !bMakeThemWalkable) // if its a enemy then it should not rotate on closing gap, enemies have their own ways to look at player
                HelpUtils.RotateTowardsTarget(transform, _targetPosition, 240f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionRadius);
    }
}
