using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCEntity))]
public class NPCScriptEditor : Editor
{
    SerializedObject targetObject;
    SerializedProperty npcBehaviour;
    SerializedProperty npcName;
    SerializedProperty npcActivity;
    SerializedProperty sRandomDialogLinesArray;   
    SerializedProperty patrolPointsArray;
    NPCEntity npc;

    public void OnEnable()
    {
        targetObject = new SerializedObject(target);
        npc = (NPCEntity)target;
        npcBehaviour = targetObject.FindProperty("npcBehaviour");
        npcName = targetObject.FindProperty("sNpcName");
        npcActivity = targetObject.FindProperty("npcActivity");
        sRandomDialogLinesArray = targetObject.FindProperty("sRandomDialogs");
        patrolPointsArray = targetObject.FindProperty("tPatrolPoints");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((NPCEntity)target), typeof(NPCEntity), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(npcName);
        EditorGUILayout.PropertyField(npcBehaviour);
        EditorGUILayout.PropertyField(npcActivity);        

        if (npc.npcActivity != NPCActivities.IDLE && npc.npcActivity != NPCActivities.SLEEPING)
        {
            npc.fSpeed = EditorGUILayout.FloatField("Speed", npc.fSpeed);
            switch (npc.npcActivity)
            {
                case NPCActivities.MOVE_RANDOMLY:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 65f;
                    npc.fWalkTime = EditorGUILayout.FloatField("Walk Time", npc.fWalkTime);
                    EditorGUIUtility.labelWidth = 60f;
                    npc.fWaitTime = EditorGUILayout.FloatField("Wait Time", npc.fWaitTime);
                    EditorGUILayout.EndHorizontal();
                    break;
                case NPCActivities.PATROLLING:
                    npc.fWaitTime = EditorGUILayout.FloatField("Wait Time", npc.fWaitTime);
                    EditorGUILayout.PropertyField(patrolPointsArray, true);
                    EditorGUIUtility.labelWidth = 150f;
                    npc.bReveseDirection = EditorGUILayout.Toggle("Reverse Direction At End", npc.bReveseDirection);
                    break;
            }
        }

        EditorGUILayout.PropertyField(sRandomDialogLinesArray);

        targetObject.ApplyModifiedProperties();
    }
}
