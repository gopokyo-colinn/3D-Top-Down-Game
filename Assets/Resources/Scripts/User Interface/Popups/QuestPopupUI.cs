using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestPopupUI : Popup
{
    public QuestElement prefabQuestElement;
    public Transform questsContainer;
    public Transform objectivesContainer;
    public TextMeshProUGUI txtQuestDescription;
    public TextMeshProUGUI txtQuestRewards;

    List<QuestElement> lstQuestElements;
    List<QuestElement> lstObjectiveElements;

    QuestElement selectedQuestElement;

    public void Start()
    {
        lstQuestElements = new List<QuestElement>();
        lstObjectiveElements = new List<QuestElement>();
        UpdateQuestsUI();
    }

    public void UpdateQuestsUI()
    {
        RemoveAllElements();
        SetQuestElements();
    }
    public void SetQuestElements()
    {
        foreach (var _quest in QuestManager.Instance.dictMainQuests)
        {
            QuestElement _qE = Instantiate(prefabQuestElement, questsContainer);
            _qE.SetElement(_quest.Value, delegate { SetSelected(_qE); });
            lstQuestElements.Add(_qE);
        }
        if(lstQuestElements.Count > 0)
            selectedQuestElement = lstQuestElements[0];
    }
    public void RemoveAllElements()
    {
        if(lstQuestElements != null)
        {
            if(lstQuestElements.Count > 0)
            {
                for (int i = 0; i < lstQuestElements.Count; i++)
                {
                    Destroy(lstQuestElements[i].gameObject);
                }
                lstQuestElements.Clear();
            }
        }
        else
        {
            lstQuestElements = new List<QuestElement>();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdateQuestsUI();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            RemoveAllElements();
        }
    }

    public void SetSelected(QuestElement _element)
    {
        selectedQuestElement.SetSelectedElement(false);

        selectedQuestElement = _element;

        if (selectedQuestElement)
        {
            selectedQuestElement.SetSelectedElement(true);
            SetQuestDetails();
        }

    }

    public void SetQuestDetails()
    {
        txtQuestDescription.text = selectedQuestElement.myQuest.sQuestDescription;
        txtQuestRewards.text = selectedQuestElement.myQuest.sRewards;

        if(lstObjectiveElements == null)
        {
            lstObjectiveElements = new List<QuestElement>();
        }

        if(lstObjectiveElements.Count > 0)
        {
            for (int i = 0; i < lstObjectiveElements.Count; i++)
            {
                Destroy(lstObjectiveElements[i].gameObject);
            }
            lstObjectiveElements.Clear();
        }

        for (int i = 0; i < selectedQuestElement.myQuest.qGoals.Length; i++)
        {
            QuestElement _qE = Instantiate(prefabQuestElement, objectivesContainer);
            _qE.SetElement(selectedQuestElement.myQuest.qGoals[i].sGoalObjective);
            lstObjectiveElements.Add(_qE);
        }
    }
}
