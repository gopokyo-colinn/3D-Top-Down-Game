using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCEntity))]
public class NPCScriptEditor : Editor
{
    SerializedObject targetObject;
    SerializedProperty dialogLinesArray;
    NPCEntity npc;

    public void OnEnable()
    {
        targetObject = new SerializedObject(target);
        npc = (NPCEntity)target;
        dialogLinesArray = targetObject.FindProperty("dialogLines");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
              
        npc.behaviour = (NPCBehaviour)EditorGUILayout.EnumPopup("Behaviour of NPC", npc.behaviour);

        if(npc.behaviour != NPCBehaviour.IDLE)
        {
            npc.activity = (NPCActivities)EditorGUILayout.EnumPopup("Activities", npc.activity);
            npc.speed = EditorGUILayout.FloatField("Speed", npc.speed);
        }

        EditorGUILayout.PropertyField(dialogLinesArray, true);
        targetObject.ApplyModifiedProperties();
    }
}
