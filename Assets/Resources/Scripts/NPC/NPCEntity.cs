using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum NPCBehaviour { SIMPLE = 0, QUEST_GIVER = 1, MERCHANT = 2}
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2, FIGHTER = 3, SLEEPING = 4 }
public class NPCEntity : MonoBehaviour
{
    const float fDISTANCE_TO_GROUND = 0.2f;
    const float fDISTANCE_TO_COLIS = 1.2f;
    //public static List<NPCEntity> npcList;
    [HideInInspector]
    public string sNpcID;
    public string sNpcName = "NPC-";
    public float fSpeed;
    public DialogArrays[] sRandomDialogs; // Use this to Randomly choose a dialog  to appear instead of sFirstDialogLines
    string[] sDialogsToUse;

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

    private Quest myActiveQuest;
    private Quest[] myQuests;
    private NewQuest[] allQuests;
    private QuestNPC[] assignedQuestGoals;
    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        assignedQuestGoals = GetComponents<QuestNPC>();

        rbody.isKinematic = true;
        SetNPCBehaviour();

        if (string.IsNullOrEmpty(sNpcID))
            sNpcID = System.Guid.NewGuid().ToString();
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
    public void AddQuests()
    {
        if (npcBehaviour == NPCBehaviour.QUEST_GIVER)
        {
            allQuests = GetComponents<NewQuest>().ToArray();
            myQuests = new Quest[allQuests.Length];
            for (int i = 0; i < allQuests.Length; i++)
            {
                myQuests[i] = allQuests[i].quest;
                if (myQuests[i] != null)
                {
                    if (myQuests[i].eQuestType == QuestType.MAINQUEST)
                    {
                        foreach (var item in QuestManager.Instance.dictMainQuests)
                        {
                            if (item.Value.sQuestID == myQuests[i].sQuestID)
                            {
                                myQuests[i] = item.Value;
                                break;
                            }
                        }
                    }
                    else if (myQuests[i].eQuestType == QuestType.SIDEQUEST)
                    {
                        foreach (var item in QuestManager.Instance.dictMainQuests)
                        {
                            if (item.Value.sQuestID == myQuests[i].sQuestID)
                            {
                                myQuests[i] = item.Value;
                                break;
                            }
                        }
                    }
                }
                foreach (var item in QuestManager.Instance.dictCompletedQuests)
                {
                    if (item.Value.sQuestID == myQuests[i].sQuestID)
                    {
                        myQuests[i] = null;
                        break;
                    }
                }
            }
        }
    }
    public void ActivateQuest()
    {
        myActiveQuest.bIsActive = true;
        myActiveQuest.Initialize(GameController.Instance.player);
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
                AddQuests();
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
        if(npcBehaviour == NPCBehaviour.QUEST_GIVER)
        {
            for (int i = 0; i < myQuests.Length; i++)
            {
                if (myQuests[i] != null)
                {
                    if (myQuests[i].IsCompleted())
                    {
                        //sDialogsToUse = sFirstDialogLines.ToArray();
                        sDialogsToUse = SelectRandomDialog();
                        continue;
                    }
                    else
                    {
                        if (!myQuests[i].bIsActive)
                        {
                            myActiveQuest = myQuests[i];
                            sDialogsToUse = allQuests[i].QuestStartDialog().ToArray();
                            break;
                        }
                        else
                        {
                            if (myQuests[i].AllGoalsCompleted())
                            {
                                sDialogsToUse = allQuests[i].QuestFinishedDialog().ToArray();
                                break;
                            }
                            else
                            {
                                if (myQuests[i].CheckQuestProgress())
                                {
                                    sDialogsToUse = allQuests[i].QuestFinishedDialog().ToArray();
                                    break;
                                }
                                else
                                {
                                    sDialogsToUse = allQuests[i].QuestInProgressDialog().ToArray();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (assignedQuestGoals != null)
        {
            CheckNpcAssignedTalkingGoals();
        }

        PopupUIManager.Instance.dialogBoxPopup.SetQuestNPC(this);
        PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogsToUse);
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
    public void CheckNpcAssignedTalkingGoals()
    {
        for (int i = 0; i < assignedQuestGoals.Length; i++)
        {
            if (assignedQuestGoals[i].QuestGoalCheck())
            {
                sDialogsToUse = assignedQuestGoals[i].sQuestDialog.ToArray();
                break;
            }
            else
            {
                //sDialogsToUse = sFirstDialogLines.ToArray();
                sDialogsToUse = SelectRandomDialog();
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
        return myActiveQuest;
    }
    public string[] SelectRandomDialog()
    {
        int _iRandom = Random.Range(0, sRandomDialogs.Length);
        return sRandomDialogs[_iRandom].sDialogLines.ToArray();
    }
}
