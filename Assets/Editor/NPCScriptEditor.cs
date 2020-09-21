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
        dialogLinesArray = targetObject.FindProperty("dialogLines");
        patrolPointsArray = targetObject.FindProperty("patrolPoints");
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
            npc.speed = EditorGUILayout.FloatField("Speed", npc.speed);
            switch (npc.activity)
            {
                case NPCActivities.MOVE_RANDOMLY:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 65f;
                    npc.walkTime = EditorGUILayout.FloatField("Walk Time", npc.walkTime);
                    EditorGUIUtility.labelWidth = 60f;
                    npc.waitTime = EditorGUILayout.FloatField("Wait Time", npc.waitTime);
                    EditorGUILayout.EndHorizontal();
                    break;
                case NPCActivities.PATROLLING:
                    npc.waitTime = EditorGUILayout.FloatField("Wait Time", npc.waitTime);
                    EditorGUILayout.PropertyField(patrolPointsArray, true);
                    EditorGUIUtility.labelWidth = 150f;
                    npc.reveseDirection = EditorGUILayout.Toggle("Reverse Direction At End", npc.reveseDirection);
                    break;
            }
        }

        EditorGUILayout.PropertyField(dialogLinesArray, true);
        targetObject.ApplyModifiedProperties();
    }
}
