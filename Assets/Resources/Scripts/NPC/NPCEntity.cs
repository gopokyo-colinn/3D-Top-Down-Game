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
    public string sNpcID = System.Guid.NewGuid().ToString();
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
    private bool bCanMove;
    private bool bIsMoving = true;
    private bool bIsInteracting;
    private bool bDialogCheck;
    private bool bDirReverse;
    public bool bReveseDirection;
    private int iPatrolPos = 0;
    // Quest Variables
    private Quest myActiveQuest;
    private AddNewQuest[] myQuestsLst;
    private NPCAssignedQuestGoal[] assignedQuestGoals;
    // Walk Area Variables
    public float fMaxWalkingDistance = 20;
    private Vector3 startPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        assignedQuestGoals = GetComponentsInChildren<NPCAssignedQuestGoal>();

        if(assignedQuestGoals.Length <= 0)
        {
            assignedQuestGoals = null;
        }

        //rbody.isKinematic = true;
        SetNPCBehaviour();

        if (string.IsNullOrEmpty(sNpcID))
            sNpcID = System.Guid.NewGuid().ToString();

        startPosition = transform.position;

        gameObject.layer = LayerMask.NameToLayer("Npc");
    }
    void Update()
    {
       // SetRbodyAccToGroundCheck();
        CheckForDialogToFinish();
        StayInWalkingArea();
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
            myQuestsLst = GetComponents<AddNewQuest>().ToArray();
        }
    }
    public void ActivateQuest()
    {
        myActiveQuest.Initialize();
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
                StayInWalkingArea();
                if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
                {
                    bIsMoving = false;
                    StartCoroutine(ChangeBoolAfter((bool b) => { bCanMove = b; }, false, fWaitTime));
                }

                rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
            }
        }
    }// TODO: Add Moving Area
    public void Patrolling()
    {
        if (bIsMoving)
        {
            lastDirection = (tPatrolPoints[iPatrolPos].position - transform.position).normalized;

            //HelperFunctions.RotateTowardsTarget(transform, lastDirection);

            if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
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
            if (!HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
            {
                transform.forward = new Vector3(lastDirection.x, transform.forward.y, lastDirection.z);
                rbody.velocity = transform.forward * fSpeed * Time.fixedDeltaTime;
            }
           
        }
    }
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
        if(npcActivity == NPCActivities.IDLE)
        {
            rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
    public void SetDialogWithQuest()
    {
        bIsInteracting = true;
        bDialogCheck = true;

        if(npcBehaviour == NPCBehaviour.QUEST_GIVER)
        {
            SetActiveQuest(); // Setting myActive Quest

            if (myActiveQuest == null) // if my active quest is null then it checks if it has assigned active goal, if there is a goal then
            {                          // then it sets dialog acc to that goal else it checks about this npc's quest
                if (!CheckNpcAssignedTalkingGoals())
                {
                    SetQuestDialogToUse();
                }
            }
            else if (myActiveQuest.GetQuestActive()) // if this npc has an active quest then it prioritize its quest than other assigned goals
            {
                SetQuestDialogToUse();
            }
            else
            {
                if (!CheckNpcAssignedTalkingGoals()) // if quest not active then it prioritize any active assigned goals
                {
                    SetQuestDialogToUse();
                }
            }
        }
        else
        {
            if (!CheckNpcAssignedTalkingGoals())
            {
                sDialogsToUse = SelectRandomDialog();
            }
        }

        PopupUIManager.Instance.dialogBoxPopup.SetQuestNPC(this);
        PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogsToUse);
    }
    public void SetQuestDialogToUse()
    {
        for (int i = 0; i < myQuestsLst.Length; i++)
        {
            if (myActiveQuest == null) // if this quest is null just continue to next one
            {
                sDialogsToUse = SelectRandomDialog();
                continue;
            }
            else
            {
                if (myActiveQuest.GetQuestCompleted()) // if this is completed, continue to nextone
                {
                    sDialogsToUse = SelectRandomDialog();
                    continue;
                }
                else
                {
                    if (!myActiveQuest.GetQuestActive()) // means quest is not active or started, or not there in quest manager
                    {
                        if (myActiveQuest.eQuestType == QuestType.MAINQUEST) // Initialize quest directly only if it is a main quest, else ask for response.
                        {
                            ActivateQuest();
                        }
                        sDialogsToUse = myQuestsLst[i].QuestStartDialog().ToArray();
                        break;
                    }
                    else
                    {
                        if (myActiveQuest.CheckQuestProgress()) // quest is active and checking its progress to check 
                        {
                            sDialogsToUse = myQuestsLst[i].QuestFinishedDialog().ToArray();
                            break;
                        }
                        else // The quest is in progress
                        {
                            sDialogsToUse = myQuestsLst[i].QuestInProgressDialog().ToArray();
                            break;
                        }
                    }
                }
            }
        }
    }
    public void SetActiveQuest()
    {
        for (int i = 0; i < myQuestsLst.Length; i++)
        {
            myActiveQuest = QuestManager.Instance.GetQuestByID(myQuestsLst[i].quest);
            if (myActiveQuest != null)
                break;                
        }
    }
    public void SetRbodyAccToGroundCheck()
    {
        if (HelpUtils.Grounded(transform, fDISTANCE_TO_GROUND))
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
        float _fWalking = (rbody.velocity.magnitude > (Vector3.one / 2).magnitude) ? 1 : 0;
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
    public bool CheckNpcAssignedTalkingGoals()
    {
        if(assignedQuestGoals != null) // right now the npc only prefers the latest goal its assigned -----
        {
            for (int i = 0; i < assignedQuestGoals.Length; i++)
            {
                if (assignedQuestGoals[i].IsFinished()) // it checks if this goal is done already, then check for the latest one
                {
                    sDialogsToUse = assignedQuestGoals[i].sQuestDialog.ToArray();

                    if(i == assignedQuestGoals.Length - 1) // if its the last assigned goal then, it should check if the mission exist and return on based of that 
                    {
                        if (assignedQuestGoals[i].QuestGoalCheck())
                        {
                            return true;
                        }
                        return false;
                    }
                    continue;
                }
                if (assignedQuestGoals[i].QuestGoalCheck()) //this is to check if the assigned goal is not finished yet
                {
                    sDialogsToUse = assignedQuestGoals[i].sQuestDialog.ToArray();
                    return true;
                }
                else
                {
                    continue;
                }
            }
        }
        sDialogsToUse = SelectRandomDialog();
        return false;
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

        if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
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
    public void StayInWalkingArea()
    {
        if ((transform.position - startPosition).sqrMagnitude > fMaxWalkingDistance)
        {
            transform.forward = (startPosition - transform.position).normalized;
           // HelpUtils.RotateTowardsTarget(transform, startPosition, Random.Range(80f, 120));
        }
    }
}

