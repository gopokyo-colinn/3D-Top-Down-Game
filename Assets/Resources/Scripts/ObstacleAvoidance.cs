using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    Rigidbody rb;
    public float fSpeed = 10;
    GameObject go;
    bool bIsHit;
    public float fAvoidanceThreshold = 4f;
    public float fDetectionSize = 2f;
    public bool bCanAvoidLargeScaleObstacles;
    Vector3 direction;
    Vector3 newVelocity = Vector3.zero;
    Vector3 tHeadOffset = new Vector3(0,.5f,0);
    public bool bMakeThemWalkable;
    bool bIsEnemy;
    Collider[] _colliders;
    // Start is called before the first frame update
    void Start()
    {
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
            if (go != null)
                CollisionAvoidance(go.transform.position);
        }
    }

    bool GetObjectHit()
    {
        if(bIsEnemy) // if its enemy then it should not avoid collision with player
            _colliders = Physics.OverlapSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionSize, LayerMask.GetMask("Default", "Enemy", "NPC"));
        else
            _colliders = Physics.OverlapSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionSize, LayerMask.GetMask("Default", "Player", "Enemy", "NPC"));

        foreach (var _coli in _colliders)
        {
            if (_coli.isTrigger || _coli.gameObject == gameObject)
            {
                continue;
            }
            else
            {
                if(go != _coli.gameObject)
                    go = _coli.gameObject;
                return true;
            }
        }
        return false;
    }
    void CollisionAvoidance(Vector3 _targetPosition)
    {
        Vector3 distanceVector = _targetPosition - transform.position;
        float fLength = distanceVector.sqrMagnitude;
        
        if (fLength < fAvoidanceThreshold)
        {
            Vector3 adjVector = distanceVector.normalized;
            float difference = fAvoidanceThreshold - fLength;
            adjVector = adjVector * difference;
            newVelocity = (rb.velocity - adjVector).normalized;
            //newVelocity.y = 0;

            if (bCanAvoidLargeScaleObstacles)
            {
                float xVel = transform.InverseTransformDirection(newVelocity).x;
                if (xVel > 0)
                    rb.velocity = newVelocity + transform.right * fSpeed * Time.fixedDeltaTime;
                else
                    rb.velocity = newVelocity - transform.right * fSpeed  * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = newVelocity * fSpeed * Time.fixedDeltaTime;
            }

            if (!bIsEnemy) // if its a enemy then it should not rotate on closing gap, enemies have their own ways to look at player
                HelpUtils.RotateTowardsTarget(transform, _targetPosition, 180f);
        }
        else
        {
            if (bCanAvoidLargeScaleObstacles)
            {
                if (bIsHit)
                {
                  //  float xVel = transform.InverseTransformDirection(rb.velocity).x;
                  //  if (xVel > 0)
                        rb.velocity = transform.right * fSpeed * Time.fixedDeltaTime;
                  //  else
                   //     rb.velocity = -transform.right * fSpeed * Time.fixedDeltaTime;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + tHeadOffset + transform.forward / 2, fDetectionSize);
    }
}
