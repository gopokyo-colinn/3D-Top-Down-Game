using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCBehaviour { IDLE = 0, QUEST_GIVER = 1, MERCHANT = 2, ACTIVITY_DOER = 3 }
public enum NPCActivities { IDLE = 0, MOVE_RANDOMLY = 1, PATROLLING = 2, FIGHTER = 3, SLEEPING = 4 }
public class NPCEntity : MonoBehaviour
{
    public static List<NPCEntity> npcList;

    [HideInInspector]
    public string npcID;
    public float speed;

    [TextArea(3, 5)]
    public string[] dialogLines;
    public NPCBehaviour behaviour;
    public NPCActivities activity;

    //if npc is quest giver
    // public AddNewQuest [] questsToAdd;
    
    //if npc is patrolling
    //public Transform[] patrolPositions;

    //if npc want to sleep
    //public Transform sleepLocation;

    private bool stayStill;
    private Animator anim;
    private Transform defaultPos;

    private bool goToDefaultPos = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform;

        anim = GetComponent<Animator>();

        if (string.IsNullOrEmpty(npcID))
            npcID = System.Guid.NewGuid().ToString();
        if (npcList == null)
            npcList = new List<NPCEntity>();
        if (!npcList.Contains(this))
            npcList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (behaviour)
        {
            case NPCBehaviour.IDLE:
                StayStill();
                break;
            case NPCBehaviour.QUEST_GIVER:
                AddQuest();
                break;
            case NPCBehaviour.MERCHANT:
                MerchantShop();
                break;
            case NPCBehaviour.ACTIVITY_DOER:
                DoActivity(activity);
                break;
        }
    }
    public void StayStill()
    {
        anim.SetTrigger("idle");
    }
    public void MoveToTarget()
    {

    }
    public void MovingRandomly()
    {

    }
    public void LookAtTarget(Transform _target)
    {
        StopAllCoroutines();
        StartCoroutine(RotateTowardsTarget(_target));
    }
    public void DoActivity(NPCActivities _activity)
    {

    }
    public void AddQuest()
    {

    }
    public void MerchantShop()
    {

    }

    // Enumerators
    IEnumerator RotateTowardsTarget(Transform _target)
    {
        var targetRotation = Quaternion.LookRotation(_target.position - transform.position);
        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.05f);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("It Finished");
    }
    IEnumerator RotateToDefaultPosition(Transform _target)
    {
        var targetRotation = Quaternion.LookRotation(_target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.05f);
        yield return new WaitForSeconds(1f);
    }
}
