using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAddQuest : MonoBehaviour
{
    // Start is called before the first frame update
    public bool bAddOnStart;
    public AddNewQuest questToAdd;
    public AddNewQuest nextQuest;
    void Start()
    {
        if (bAddOnStart)
        {
            StartCoroutine(HelpUtils.WaitForSeconds(delegate { AddQuest(); }, 1f));
        }
    }
    public void AddQuest()
    {
        if(questToAdd)
            questToAdd.quest.Initialize();
    }
    public void AddNextQuest()
    {
        if (nextQuest)
        {
            nextQuest.quest.Initialize();
        }
    }

}
