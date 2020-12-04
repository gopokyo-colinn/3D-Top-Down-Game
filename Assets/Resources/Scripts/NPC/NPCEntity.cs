using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NPCBehaviour { SIMPLE = 0, QUEST_GIVER = 1, MERCHANT = 2}
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2, FIGHTER = 3, SLEEPING = 4 }
public class NPCEntity : MonoBehaviour
{
    const float fDISTANCE_TO_GROUND = 0.1f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    public static List<NPCEntity> npcList;
    [HideInInspector]
    public string sNpcID;
    public float fSpeed;
    [TextArea(3, 5)]
    public string[] sDialogLines;
    public NPCBehaviour behaviour;
    public NPCActivities activity;
    public Transform[] tPatrolPoints;
    private Animator anim;
    private Rigidbody rbody;
    public float fWalkTime;
    public float fWaitTime;
    private Vector3 randomVector;
    private Vector3 lastDirection;
    private bool isMovingRandomly = false;
    private bool bCanMove = true;
    private bool bIsInteracting;
    private bool bDialogCheck = false;
    private bool bDirReverse;
    public bool bReveseDirection;
    private int iPatrolPos = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        rbody.isKinematic = true;

        if (string.IsNullOrEmpty(sNpcID))
            sNpcID = System.Guid.NewGuid().ToString();
        if (npcList == null)
            npcList = new List<NPCEntity>();
        if (!npcList.Contains(this))
            npcList.Add(this);
    }
    void Update()
    {
        
        SetRbodyAccToGroundCheck();
        CheckForDialogToFinish();
        SetAnimations();
        switch (behaviour)
        {
            case NPCBehaviour.SIMPLE:
                break;
            case NPCBehaviour.QUEST_GIVER:
                AddQuest();
                break;
            case NPCBehaviour.MERCHANT:
                MerchantShop();
                break;
        }
    }
    private void FixedUpdate()
    {
        switch (behaviour)
        {
            case NPCBehaviour.SIMPLE:
                DoActivity(activity);
                break;
            case NPCBehaviour.QUEST_GIVER:
                DoActivity(activity);
                break;
            case NPCBehaviour.MERCHANT:
                DoActivity(activity);
                break;
        }
    }

    /// Behaviours
    public void StayStill()
    {
        rbody.velocity = VectorZero();
    }
    public void DoActivity(NPCActivities _activity)
    {
        if (!bIsInteracting)
        {
            //Debug.Log("I m doing some activity");
            switch (activity)
            {
                case NPCActivities.IDLE:
                    StayStill();
                    break;
                case NPCActivities.MOVE_RANDOMLY:
                    MovingRandomly();
                    break;
                case NPCActivities.PATROLLING:
                    Patrolling();
                    break;
            }
        }      
    }
    public void AddQuest()
    {
        Debug.Log("I will give you a quest");
    }
    public void MerchantShop()
    {
        Debug.Log("I m a Merchant");
    }

    /// Activities
    public void MovingRandomly()
    {
        if (!isMovingRandomly)
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
                    StartCoroutine(ChangeBoolAfter((bool b) =>{ isMovingRandomly = b; }, false, fWaitTime));
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
    }// Add Moving Area
    public void Patrolling()
    {
        if (bCanMove)
        {
            lastDirection = (tPatrolPoints[iPatrolPos].position - transform.position).normalized;

            //HelperFunctions.RotateTowardsTarget(transform, lastDirection);

            if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
            {
                bCanMove = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bCanMove = b; }, true, fWaitTime));
            }

            if ((transform.position - tPatrolPoints[iPatrolPos].position).sqrMagnitude <= 1f)
            {
                bCanMove = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bCanMove = b; }, true, fWaitTime));

                if (bDirReverse)
                    iPatrolPos--;
                else
                    iPatrolPos++;
            }
            if(iPatrolPos >= tPatrolPoints.Length)
            {
                if (bReveseDirection)
                {
                    iPatrolPos--;
                    bDirReverse = true;
                }
                else
                    iPatrolPos = 0;
            }
            else if(iPatrolPos == 0)
            {
                if (bDirReverse)
                {
                    bDirReverse = false;
                }
            }
            if (!HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
            {
                transform.forward = new Vector3(lastDirection.x, transform.forward.y, lastDirection.z);
                rbody.MovePosition(transform.position + (transform.forward * fSpeed * Time.fixedDeltaTime));
            }
           
        }
    }
    /*//public void FindPathToLocation()
    //{
    //    bool bf = Physics.Raycast(startPos, Vector3.forward, 5f);
    //    bool bb = Physics.Raycast(startPos, Vector3.forward * -1, 5f);
    //    bool br = Physics.Raycast(startPos, Vector3.right, 5f);
    //    bool bl = Physics.Raycast(startPos, Vector3.right * -1, 5f);
    //    if (bf || bb || br || bl)
    //    {
    //        Debug.DrawRay(startPos, Vector3.forward * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.forward * -1 * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.right * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.right * -1 * 5f, Color.red);
    //        startPos = new Vector3(startPos.x + 0.2f, startPos.y, startPos.z);
    //    }
    //    else
    //    {
    //        Debug.DrawRay(startPos, Vector3.forward * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.forward * -1 * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.right * 5f, Color.red);
    //        Debug.DrawRay(startPos, Vector3.right * -1 * 5f, Color.red);
    //    }
    //}
    */

    // Setter Functions
    public void SetDialog()
    {
        bIsInteracting = true;
        bDialogCheck = true;
        PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogLines);
    }
    public void SetRbodyAccToGroundCheck()
    {
        if (HelperFunctions.Grounded(transform, fDISTANCE_TO_GROUND))
        {
            rbody.isKinematic = true;
        }
        else
            rbody.isKinematic = false;
    }
    public void LookAtTarget(Transform _target)
    {
        StopAllCoroutines();
        isMovingRandomly = false;
        bCanMove = false;
        StartCoroutine(RotateTowardsTarget(_target));
    }
    public void SetAnimations()
    {
        float _fWalking = (rbody.velocity == Vector3.zero) ? 0 : 1;
        anim.SetFloat("m_speed", _fWalking);
    }

    /// Checker Functions
    public void CheckForDialogToFinish()
    {
        if (bIsInteracting)
        {
            if (!PopupUIManager.Instance.dialogBoxPopup.GetDialogInProgress())
            {
                if (bDialogCheck)
                {
                    StartCoroutine(ChangeBoolAfter((bool b) => { bIsInteracting = b; bCanMove = true; }, false, 1f));
                    bDialogCheck = false;
                }
            }
        }
    }

    /// Enumerators
    IEnumerator RotateTowardsTarget(Transform _target)
    {
        var targetRotation = Quaternion.LookRotation(_target.position - transform.position);
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("It Finished");
    }
    IEnumerator MoveRandom()
    {
        randomVector = new Vector3(Random.Range(1, -1), 0, Random.Range(-1, 1));

        if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        {
            randomVector *= -1; //new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        }

        bCanMove = true;
        isMovingRandomly = true;
        yield return new WaitForSeconds(fWalkTime);
        bCanMove = false;
        yield return new WaitForSeconds(fWaitTime);
        isMovingRandomly = false;
    }
    IEnumerator ChangeBoolAfter(System.Action<bool> _callBack,bool _setBool , float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        StopAllCoroutines();
    }

    /// Other Helper Functions
    public Vector3 VectorZero()
    {
        return new Vector3(0, rbody.velocity.y, 0);
    }
}
