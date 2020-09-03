using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCBehaviour { SIMPLE = 0, QUEST_GIVER = 1, MERCHANT = 2, FIGHTER = 3, ACTIVITY_DOER = 4 }
public class NPCEntity : MonoBehaviour
{
    [HideInInspector]
    public string npcID;
    public float speed;
    public string[] dialogLines;
    public bool stayStill;
    public NPCBehaviour behaviour;

    public static List<NPCEntity> npcList;
    // Start is called before the first frame update
    void Start()
    {
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
            case NPCBehaviour.SIMPLE:
                break;
            case NPCBehaviour.QUEST_GIVER:
                break;
            case NPCBehaviour.MERCHANT:
                break;
            case NPCBehaviour.FIGHTER:
                break;
            case NPCBehaviour.ACTIVITY_DOER:
                break;
        }
    }
    public void StayStill()
    {

    }
    public void MoveToTarget()
    {

    }
    public void MovingRandomly()
    {

    }
    public void LookAtPlayer()
    {

    }
    public void DoTask()
    {

    }
}
