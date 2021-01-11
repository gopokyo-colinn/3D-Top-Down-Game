using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NPCBehaviour { SIMPLE = 0, QUEST_GIVER = 1, MERCHANT = 2}
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2, FIGHTER = 3, SLEEPING = 4 }
public class NPCEntity : MonoBehaviour
{
    const float fDISTANCE_TO_GROUND = 0.2f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    //public static List<NPCEntity> npcList;
    [HideInInspector]
    public string sNpcID;
    public float fSpeed;
    [TextArea(2, 3)]
    public string[] sDefaultDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
        "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestStartDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
        "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestInProgressDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
        "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestEndDialogLines;

    string[] sDialogToUse;

    public NPCBehaviour npcBehaviour;
    public NPCActivities npcActivity;
    public Transform[] tPatrolPoints;
    private Animator anim;
    private Rigidbody rbody;
    public float fWalkTime;
    public float fWaitTime;
    private Vector3 randomVector;
    private Vector3 lastDirection;
    private bool bCanMove = false;
    private bool bIsMoving = true;
    private bool bIsInteracting;
    private bool bDialogCheck = false;
    private bool bDirReverse;
    public bool bReveseDirection;
    private int iPatrolPos = 0;

    private Quest myQuest;

    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        rbody.isKinematic = true;
        SetNPCBehaviour();

        if (string.IsNullOrEmpty(sNpcID))
            sNpcID = System.Guid.NewGuid().ToString();
        //if (npcList == null)
        //    npcList = new List<NPCEntity>();
        //if (!npcList.Contains(this))
        //    npcList.Add(this);
    }
    void Update()
    {
        SetRbodyAccToGroundCheck();
        CheckForDialogToFinish();
        SetAnimations();
    }
    private void FixedUpdate()
    {
        switch (npcBehaviour)
        {
            case NPCBehaviour.SIMPLE:
                DoActivity(npcActivity);
                break;
            case NPCBehaviour.QUEST_GIVER:
                DoActivity(npcActivity);
                break;
            case NPCBehaviour.MERCHANT:
                DoActivity(npcActivity);
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
            switch (npcActivity)
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

    /// NPC QUESTING SYSTEM                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
    public void AddQuest()
    {
        myQuest = GetComponent<QuestGiver>().quest;

        if (myQuest.eQuestType == QuestType.MAINQUEST)
        {
            foreach (var item in QuestManager.Instance.dictMainQuests)
            {
                if(item.Value.sQuestID == myQuest.sQuestID)
                {
                    myQuest = item.Value;
                    break;
                }
            }
        }
        else if (myQuest.eQuestType == QuestType.SIDEQUEST)
        {
            foreach (var item in QuestManager.Instance.dictMainQuests)
            {
                if (item.Value.sQuestID == myQuest.sQuestID)
                {
                    myQuest = item.Value;
                    break;
                }
            }
        }

        foreach (var item in QuestManager.Instance.dictCompletedQuests)
        {
            if (item.Value.sQuestID == myQuest.sQuestID)
            {
                myQuest = null;
                break;
            }
        }
    }
    public string[] QuestStartDialog()
    {
        if(myQuest.eQuestType == QuestType.MAINQUEST)
        {
            if (!QuestManager.Instance.dictMainQuests.ContainsValue(myQuest))
            {
                myQuest.bIsActive = true;
                myQuest.Initialize(GameController.Instance.player);
            }
        }
        // else is its a side quest it asks for user response below
        return sQuestStartDialogLines;
    }
    public string[] QuestInProgressDialog()
    {
        return sQuestInProgressDialogLines;
    }
    public string[] QuestFinishedDialog()
    {
        myQuest.GiveReward();
        myQuest.qGoal = null;
        return sQuestEndDialogLines;
    }
    public void ActivateQuest()
    {
        myQuest.bIsActive = true;
        myQuest.Initialize(GameController.Instance.player);
    }
    // NPC SHOP SYSTEM
    public void MerchantShop()
    {
        Debug.Log("I m a Merchant");
    }

    /// Activities
    public void MovingRandomly()
    {
        if (!bCanMove)
        {
            StopAllCoroutines();
            StartCoroutine(SetRandomDirection());
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
        }
    }// Add Moving Area
    public void Patrolling()
    {
        if (bIsMoving)
        {
            lastDirection = (tPatrolPoints[iPatrolPos].position - transform.position).normalized;

            //HelperFunctions.RotateTowardsTarget(transform, lastDirection);

            if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
            {
                bIsMoving = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bIsMoving = b; }, true, fWaitTime));
            }

            if ((transform.position - tPatrolPoints[iPatrolPos].position).sqrMagnitude <= 1f)
            {
                bIsMoving = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { bIsMoving = b; }, true, fWaitTime));

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
    public void SetNPCBehaviour()
    {
        switch (npcBehaviour)
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
    public void SetDialog()
    {
        bIsInteracting = true;
        bDialogCheck = true;

        if (myQuest == null || myQuest.bIsCompleted)
        {
            sDialogToUse = sDefaultDialogLines;
        }
        else if (myQuest != null)
        {
            if (!myQuest.bIsActive)
            {
                sDialogToUse = QuestStartDialog();
            }
            else
            {
                if (myQuest.qGoal.bIsFinished)
                {
                    sDialogToUse = QuestFinishedDialog();
                }
                else
                {
                    sDialogToUse = QuestInProgressDialog();
                }
            }
        }
        
        PopupUIManager.Instance.dialogBoxPopup.SetQuestNPC(this);

        PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogToUse);
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
        bCanMove = false;
        bIsMoving = false;
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
                    StartCoroutine(ChangeBoolAfter((bool b) => { bIsInteracting = b; bIsMoving = true; }, false, 1f));
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
    IEnumerator SetRandomDirection()
    {
        randomVector = new Vector3(Random.Range(1f, -1f), 0, Random.Range(1f, -1f)); // For transfom forward without rotation

        if (HelperFunctions.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        {
            transform.forward *= -1;
        }
        else
        {
            if (randomVector != Vector3.zero)
                transform.forward = randomVector;
            else
                transform.forward = new Vector3(0.1f, randomVector.y, 0.1f); // Fixed bug for look rotation (and speedy or no movement)
        }

        bIsMoving = true;
        bCanMove = true;
        yield return new WaitForSeconds(fWalkTime);
        bIsMoving = false;
        yield return new WaitForSeconds(fWaitTime);
        bCanMove = false;
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
    public Quest GetQuest()
    {
        return myQuest;
    }
}
