using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestPopupUI : Popup
{
    public Transform questsContainer;
    public Transform objectivesContainer;
    public TextMeshProUGUI txtQuestDescription;
    public TextMeshProUGUI txtQuestRewards;

    List<QuestElement> lstQuestElements;
    List<QuestElement> lstObjectiveElements;
    List<QuestElement> lstlabelElements;

    QuestElement selectedQuestElement;

    public QuestElement prefabQuestElement;
    public QuestElement prefabLabelQuestElement;

    public override void open()
    {
        base.open();
        UpdateQuestsUI();
        PopupUIManager.Instance.menuBarPopup.open();
    }
    public override void close()
    {
        base.close();
        PopupUIManager.Instance.menuBarPopup.close();
    }
    public void Start()
    {
        lstQuestElements = new List<QuestElement>();
        lstObjectiveElements = new List<QuestElement>();
        lstlabelElements = new List<QuestElement>();
        UpdateQuestsUI();
    }

    public void UpdateQuestsUI()
    {
        RemoveAllElements();
        SetQuestElements();
    }
    public void SetQuestElements()
    {
        QuestElement _mainQuestLabel = Instantiate(prefabLabelQuestElement, questsContainer);
        _mainQuestLabel.SetElement("Main Quests:");
        lstlabelElements.Add(_mainQuestLabel);

        foreach (var _quest in QuestManager.Instance.dictMainQuests)
        {
            QuestElement _qE = Instantiate(prefabQuestElement, questsContainer);
            _qE.SetElement(_quest.Value, delegate { SetSelected(_qE); });
            lstQuestElements.Add(_qE);
        }
        // TODO: maybe Add another list containing of only the label elements or just make two variables for both labels
        if(QuestManager.Instance.dictSideQuests.Count > 0)
        {
            QuestElement _sideQuestLabel = Instantiate(prefabLabelQuestElement, questsContainer);
            _sideQuestLabel.SetElement("Side Quests:");
            lstlabelElements.Add(_sideQuestLabel);
        }

        foreach (var _quest in QuestManager.Instance.dictSideQuests)
        {
            QuestElement _qE = Instantiate(prefabQuestElement, questsContainer);
            _qE.SetElement(_quest.Value, delegate { SetSelected(_qE); });
            lstQuestElements.Add(_qE);
        }
       // if(selectedQuestElement == null) // TODO: Make it so the selected element is previously selected one
        {
            if (lstQuestElements.Count > 0)
                SetSelected(lstQuestElements[0]); 
        }
    }
    public void RemoveAllElements()
    {
        for (int i = 0; i < lstlabelElements.Count; i++)
        {
            Destroy(lstlabelElements[i].gameObject);
        }
        lstlabelElements.Clear();

        if (lstQuestElements.Count > 0)
        {
            for (int i = 0; i < lstQuestElements.Count; i++)
            {
                Destroy(lstQuestElements[i].gameObject);
            }
            lstQuestElements.Clear();
        }
        if (lstObjectiveElements.Count > 0)
        {
            for (int i = 0; i < lstObjectiveElements.Count; i++)
            {
                Destroy(lstObjectiveElements[i].gameObject);
            }
            lstObjectiveElements.Clear();
        }
    }

    public void SetSelected(QuestElement _element)
    {
        if(selectedQuestElement != null)
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
            if (selectedQuestElement.myQuest.bQuestInAnyOrder)
            {
                QuestElement _qE = Instantiate(prefabQuestElement, objectivesContainer);
                _qE.SetElement(selectedQuestElement.myQuest.qGoals[i].sGoalObjective, true);

                if (selectedQuestElement.myQuest.qGoals[i].bIsFinished)
                {
                    _qE.SetFontStrikethrough();
                }
                lstObjectiveElements.Add(_qE);
            }
            else
            {
                if (selectedQuestElement.myQuest.qGoals[i].bIsActive)
                {
                    QuestElement _qE = Instantiate(prefabQuestElement, objectivesContainer);
                    _qE.SetElement(selectedQuestElement.myQuest.qGoals[i].sGoalObjective, true);

                    if (selectedQuestElement.myQuest.qGoals[i].bIsFinished)
                    {
                        _qE.SetFontStrikethrough();
                    }
                    lstObjectiveElements.Add(_qE);
                }
            }
        }
    }
}
