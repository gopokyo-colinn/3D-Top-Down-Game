using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCEntity))]
public class NPCScriptEditor : Editor
{
    SerializedObject targetObject;
    SerializedProperty dialogLinesArray;
    SerializedProperty patrolPointsArray;
    NPCEntity npc;

    public void OnEnable()
    {
        targetObject = new SerializedObject(target);
        npc = (NPCEntity)target;
        dialogLinesArray = targetObject.FindProperty("sDialogLines");
        patrolPointsArray = targetObject.FindProperty("tPatrolPoints");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((NPCEntity)target), typeof(NPCEntity), false);
        GUI.enabled = true;

        npc.behaviour = (NPCBehaviour)EditorGUILayout.EnumPopup("Behaviour of NPC", npc.behaviour);
        npc.activity = (NPCActivities)EditorGUILayout.EnumPopup("Activity", npc.activity);

        if (npc.activity != NPCActivities.IDLE && npc.activity != NPCActivities.SLEEPING)
        {
            npc.fSpeed = EditorGUILayout.FloatField("Speed", npc.fSpeed);
            switch (npc.activity)
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

        EditorGUILayout.PropertyField(dialogLinesArray, true);
        targetObject.ApplyModifiedProperties();
    }
}
