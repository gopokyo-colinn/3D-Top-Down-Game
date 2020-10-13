using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCBehaviour { SIMPLE = 0, QUEST_GIVER = 1, MERCHANT = 2}
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2, FIGHTER = 3, SLEEPING = 4 }
public class NPCEntity : MonoBehaviour
{
    const float DISTANCE_TO_GROUND = 0.1f;
    const float DISTANCE_TO_COLIS = 1.2f;

    public static List<NPCEntity> npcList;

    [HideInInspector]
    public string npcID;
    public float speed;

    [TextArea(3, 5)]
    public string[] dialogLines;
    public NPCBehaviour behaviour;
    public NPCActivities activity;

    public Transform[] patrolPoints;

    //if npc is quest giver
    // public AddNewQuest [] questsToAdd;
    //if npc want to sleep
    //public Transform sleepLocation;

    private Animator anim;
    private Rigidbody rbody;

    public float walkTime;
    public float waitTime;

    private Vector3 randomVector;
    private Vector3 lastDirection;
    private Vector3 startPos;
    
    private bool isMovingRandomly = false;
    private bool canMove = true;
    private bool isInteracting;
    private bool dialogCheck = false;
    private bool _dirReverse;

    public bool reveseDirection;

    private int patrolPos = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
        startPos = transform.position;

        rbody.isKinematic = true;

        if (string.IsNullOrEmpty(npcID))
            npcID = System.Guid.NewGuid().ToString();
        if (npcList == null)
            npcList = new List<NPCEntity>();
        if (!npcList.Contains(this))
            npcList.Add(this);
    }

    void Update()
    {
        SetRbodyAccToGroundCheck();
        CheckForDialogToFinish();

        if (!canMove)
        {
            rbody.velocity = VectorZero();
        }
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

    /// <summary>
    /// Behaviours
    /// </summary>
    public void StayStill()
    {
        anim.SetTrigger("idle");
        rbody.velocity = VectorZero();
        Debug.Log("I m idle");
    }
    public void DoActivity(NPCActivities _activity)
    {
        if (!isInteracting)
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

    /// <summary>
    /// Activities
    /// </summary>
    public void MovingRandomly()
    {       
        if (!isMovingRandomly)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRandom());
        }
        else
        {
            if (canMove)
            {
                if(CheckAheadForColi())
                {
                    canMove = false;
                    StartCoroutine(ChangeBoolAfter((bool b) =>{ isMovingRandomly = b; }, false, waitTime));
                }
                else
                {
                    transform.forward = new Vector3(randomVector.normalized.x, transform.forward.y, randomVector.normalized.z);
                    rbody.MovePosition(transform.position + randomVector.normalized * speed * Time.fixedDeltaTime);
                }
            }
        }
    }
    public void Patrolling()
    {
        if (canMove)
        {
            lastDirection = patrolPoints[patrolPos].position - transform.position;

            if (CheckAheadForColi())
            {
                canMove = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { canMove = b; }, true, waitTime));
            }

            if ((transform.position - patrolPoints[patrolPos].position).sqrMagnitude <= 1f)
            {
                canMove = false;
                StartCoroutine(ChangeBoolAfter((bool b) => { canMove = b; }, true, waitTime));
                if (_dirReverse)
                    patrolPos--;
                else
                    patrolPos++;
            }
            if(patrolPos >= patrolPoints.Length)
            {
                if (reveseDirection)
                {
                    patrolPos--;
                    _dirReverse = true;
                }
                else
                    patrolPos = 0;
            }
            else if(patrolPos == 0)
            {
                if (_dirReverse)
                {
                    _dirReverse = false;
                }   
            }

            transform.forward = new Vector3(lastDirection.x, transform.forward.y, lastDirection.z);        
            rbody.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
        }
    }
    //public void FindPathToLocation()
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

    /// <summary>
    // Setter Functions
    /// </summary>
    public void SetDialog()
    {
        isInteracting = true;
        dialogCheck = true;
        PopupUIManager.Instance.dialogBoxPopup.setDialogText(dialogLines);
    }
    public void SetRbodyAccToGroundCheck()
    {
        if (Grounded())
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
        canMove = false;
        StartCoroutine(RotateTowardsTarget(_target));
    }

    /// <summary>
    /// Checker Functions
    /// </summary>
    public bool Grounded()
    {
        // use a spherecast instead
        return Physics.Raycast(transform.position, Vector3.down, DISTANCE_TO_GROUND);
    }
    public bool CheckAheadForColi()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, DISTANCE_TO_COLIS);
    }
    public bool CheckAheadForColi(string _layerName)
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, DISTANCE_TO_COLIS, LayerMask.GetMask(_layerName));
    }
    public void CheckForDialogToFinish()
    {
        if (isInteracting)
        {
            if (!PopupUIManager.Instance.dialogBoxPopup.GetDialogInProgress())
            {
                if (dialogCheck)
                {
                    StartCoroutine(ChangeBoolAfter((bool b) => { isInteracting = b; canMove = true; }, false, 1f));
                    dialogCheck = false;
                }
            }
        }
    }

    /// <summary>
    /// Enumerators
    /// </summary>
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
        randomVector = new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        if (CheckAheadForColi())
        {
            randomVector = new Vector3(Random.Range(1f, -1f), 0, Random.Range(-1f, 1f));
        }
        transform.forward = new Vector3(randomVector.normalized.x, transform.forward.y, randomVector.normalized.z);
        //lastFacingDir = randomVector;
        canMove = true;
        isMovingRandomly = true;
        yield return new WaitForSeconds(walkTime);
        canMove = false;
        yield return new WaitForSeconds(waitTime);
        isMovingRandomly = false;
    }
    IEnumerator ChangeBoolAfter(System.Action<bool> _callBack,bool _setBool , float _time)
    {
        yield return new WaitForSeconds(_time);
        _callBack(_setBool);
        StopAllCoroutines();
    }

    /// <summary>
    /// Other Helper Functions
    /// </summary>
    public Vector3 VectorZero()
    {
        return new Vector3(0, rbody.velocity.y, 0);
    }
}
