using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestGoalType { KILL = 0, GATHER = 1, DELIVER = 2, GOTOLOCATION = 3}

[Serializable]
public class QuestGoal 
{
    public QuestGoalType eGoalType;
    public bool isFinished;
    public Transform tLocationToReach;
}
