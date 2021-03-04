using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum NPCBehaviour { SIMPLE = 0, AVOID_OBSTACLES = 1}
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2}
public class NPCEntity : MonoBehaviour
{
    const float fDISTANCE_TO_GROUND = 0.2f;
    const float fDISTANCE_TO_COLIS = 0.8f;
    const float fROTATE_SPEED = 240f;
    //public static List<NPCEntity> npcList;
    [HideInInspector]
    public string sNpcID = System.Guid.NewGuid().ToString();
    public string sNpcName = "NPC-";
    public float fSpeed;
    public DialogArrays[] sRandomDialogs; // Use this to Randomly choose a dialog  to appear instead of sFirstDialogLines
    string[] sDialogsToUse;

    public NPCBehaviour npcBehaviour;
    public NPCActivities npcActivity;
    private Animator anim;
    private Rigidbody rbody;
    public float fWalkTime;
    public float fWaitTime;
    private bool bCanMove;
    private bool bIsMoving;
    private bool bIsInteracting;
    private Vector3 randomVector;
    private Vector3 moveVector;

    private bool bCanRotate;
    private bool bDialogCheck;
    // Patrolling
    private Vector3 lastDirection;
    public Transform[] tPatrolPoints;
    public bool bReverseDirection; // it is to enable or disable reverse direction
    private bool bDirReversing; // it is actually reversing direction if the npc reaches the end point
    private int iPatrolPos = 0;
    // Quest Variables
    private Quest myActiveQuest;
    private AddNewQuest[] myQuestsLst;
    private NPCAssignedQuestGoal[] assignedQuestGoals;
    // Walk Area Variables
    public float fMaxWalkingDistance = 60;
    private Vector3 startPosition;
    private Quaternion startRotation;


    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        assignedQuestGoals = GetComponentsInChildren<NPCAssignedQuestGoal>();
        if(assignedQuestGoals.Length <= 0)
            assignedQuestGoals = null;

        AddQuestsIfAny();
        SetNPCActivities();

        startPosition = transform.position;
        startRotation = transform.rotation;

        gameObject.layer = LayerMask.NameToLayer("Npc");


        // Starting Idle Animation at random time
        AnimatorStateInfo _animState = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(_animState.fullPathHash, -1, Random.Range(0f, 1f));
    }
    void Update()
    {
        //SetRbodyAccToGroundCheck(); Maybe use it if you need npc's in action
        CheckForDialogToFinish();
        StayInWalkingArea();
        SetAnimations();
    }
    private void FixedUpdate()
    {
        if(HelpUtils.Grounded(transform, 0.25f))
        {
            DoActivity(npcActivity);
        }
    }

    /// Behaviours
  
    public void DoActivity(NPCActivities _activity)
    {
        if (!bIsInteracting)
        {
            //Debug.Log("I m doing some activity");
            switch (npcActivity)
            {
                case NPCActivities.IDLE:
                    IdleActivity();
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
    public void AddQuestsIfAny()
    {
       // if (npcBehaviour == NPCBehaviour.QUEST_GIVER)
        {
            myQuestsLst = GetComponents<AddNewQuest>().ToArray();
        }
    }
    public void ActivateQuest()
    {
        myActiveQuest.Initialize();
    }

    /// Activities
    public void IdleActivity()
    {
        rbody.velocity = HelpUtils.VectorZeroWithY(rbody);
    }
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
                    StartCoroutine(HelpUtils.WaitForSeconds(delegate { bCanMove = false; }, fWaitTime / 2)); ;
                }

                moveVector = new Vector3(transform.forward.x, rbody.velocity.y, transform.forward.z);
                rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (bCanRotate)
                {
                    HelpUtils.RotateTowards(transform, randomVector, fROTATE_SPEED);
                }
            }
        }
    }// TODO: Add Moving Area
    public void Patrolling()
    {
        if (bIsMoving)
        {
            lastDirection = (tPatrolPoints[iPatrolPos].position - transform.position).normalized;
            lastDirection.y = 0;

            if ((transform.position - tPatrolPoints[iPatrolPos].position).sqrMagnitude <= 0.2f)
            {
                if (bDirReversing)
                    iPatrolPos--;
                else
                    iPatrolPos++;

                if (iPatrolPos >= tPatrolPoints.Length)
                {
                    if (bReverseDirection)
                    {
                        iPatrolPos = tPatrolPoints.Length - 2;
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
                if (fWaitTime > 0.3f)
                {
                    bIsMoving = false;
                    StartCoroutine(HelpUtils.ChangeBoolAfter((bool b) => { bIsMoving = b; }, true, fWaitTime));
                }
                else
                    transform.LookAt(tPatrolPoints[iPatrolPos].position);
            }
            moveVector = new Vector3(lastDirection.x, rbody.velocity.y, lastDirection.z);
            rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
        }
        else
        {
            HelpUtils.RotateTowards(transform, tPatrolPoints[iPatrolPos].position, fROTATE_SPEED);
        }
    }
    // Setter Functions
    public void SetNPCActivities()
    {
        switch (npcActivity)
        {
            case NPCActivities.IDLE:
                rbody.isKinematic = true;
                break;
            case NPCActivities.PATROLLING:
                bIsMoving = true;
                rbody.isKinematic = false;
                break;
            default:
                rbody.isKinematic = false;
                break;
        } 
    }
    public bool SetDialogWithQuest()
    {
        bIsInteracting = true;
        bDialogCheck = true;

        if(myQuestsLst.Length > 0)
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
            PopupUIManager.Instance.dialogBoxPopup.SetQuestNPC(this);
            PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogsToUse);
            return true;
        }
        else
        {
            if (sRandomDialogs.Length > 0)
            {
                if (sRandomDialogs[0].sDialogLines.Length > 0)
                {
                    if (!CheckNpcAssignedTalkingGoals())
                    {
                        sDialogsToUse = SelectRandomDialog();
                        PopupUIManager.Instance.dialogBoxPopup.SetQuestNPC(this);
                        PopupUIManager.Instance.dialogBoxPopup.setDialogText(sDialogsToUse);
                        return true;
                    }
                }
            }
        }
        bIsInteracting = false;
        bDialogCheck = false;
        return false;
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
        StartCoroutine(HelpUtils.RotateTowardsTarget(transform, _target));
    }
    public void SetAnimations()
    {
        float _fWalking = (bIsMoving || rbody.velocity.sqrMagnitude != 0) ? 1 : 0;
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
                    StartCoroutine(HelpUtils.WaitForSeconds(delegate { bIsInteracting = false;}, 1f));
                    if(npcActivity == NPCActivities.IDLE)
                        StartCoroutine(HelpUtils.RotateTowardsTarget(transform, startRotation));
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
    IEnumerator SetRandomDirection()
    {
        randomVector = transform.position + new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1));// Random.insideUnitSphere * 100;
        randomVector.y = 0;

        //if (HelpUtils.CheckAheadForColi(transform, fDISTANCE_TO_COLIS))
        //{
        //    transform.forward *= -1;
        //}

        bCanMove = true;
        bIsMoving = true;
        yield return new WaitForSeconds(fWalkTime);
        bIsMoving = false;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanRotate = true;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanRotate = false;
        yield return new WaitForSeconds(fWaitTime / 3);
        bCanMove = false;
    }

    /// Other Helper Functions
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
        if(npcActivity != NPCActivities.PATROLLING)
        {
            if ((transform.position - startPosition).sqrMagnitude > fMaxWalkingDistance)
            {
                Vector3 _targetPos = (startPosition - transform.position).normalized;
                transform.forward = _targetPos;
                moveVector = new Vector3(_targetPos.x, rbody.velocity.y, _targetPos.z);
                rbody.MovePosition(transform.position + moveVector * fSpeed * Time.fixedDeltaTime);
                //rbody.velocity = moveVector * fSpeed * Time.fixedDeltaTime;
               // HelpUtils.RotateTowardsTarget(transform, startPosition, Random.Range(80f, 120));
            }
        }
    }
}

