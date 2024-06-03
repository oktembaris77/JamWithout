using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SlotTypeBehaviorManagerScript))]
public class SlotTypeBehaviorManager : Editor
{
    private void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        //Called whenever the inspector is drawn for this object.
        //DrawDefaultInspector();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("slotTypeBehavior"), true);

        if (!Application.isPlaying)
        {
            if (GUILayout.Button("Add Behavior"))
            {
                if (SlotTypeBehaviorManagerScript.itemTypeManager.CheckBehavior(SlotTypeBehaviorManagerScript.itemTypeManager.slotTypeBehavior))
                {
                    //Add ItemToolManager
                    ItemToolManager itemToolManager = GameObject.Find("Managers").GetComponent<ItemToolManager>();
                    itemToolManager.behaviors.Add(new SlotTypeBehavior(SlotTypeBehaviorManagerScript.itemTypeManager.slotTypeBehavior.fromSlotType, SlotTypeBehaviorManagerScript.itemTypeManager.slotTypeBehavior.toSlotType));
                    //Add Behaviour
                    SlotTypeBehaviorManagerScript.itemTypeManager.behaviors.Add(new SlotTypeBehavior(SlotTypeBehaviorManagerScript.itemTypeManager.slotTypeBehavior.fromSlotType, SlotTypeBehaviorManagerScript.itemTypeManager.slotTypeBehavior.toSlotType));

                    //Save in Editor
                    EditorUtility.SetDirty(itemToolManager);
                    Undo.RecordObject(itemToolManager, "addBehavior");
                    SaveSlotTypeBehaviorManager();
                }
            }
        }
        else GUILayout.Label("Only in Pause Mode");

        for (int i = 0; i < SlotTypeBehaviorManagerScript.itemTypeManager.behaviors.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

           // SlotTypeBehavior slotTypeBehavior = SlotTypeBehaviorManagerScript.itemTypeManager.behaviors[i];
            GUILayout.Label("FROM " + SlotTypeBehaviorManagerScript.itemTypeManager.behaviors[i].fromSlotType + " TO " + SlotTypeBehaviorManagerScript.itemTypeManager.behaviors[i].toSlotType);
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Remove Behavior"))
                {
                    //Destroy Behaviour
                    ItemToolManager itemToolManager = GameObject.Find("Managers").GetComponent<ItemToolManager>();
                    itemToolManager.behaviors.RemoveAt(i);
                    SlotTypeBehaviorManagerScript.itemTypeManager.behaviors.RemoveAt(i);

                    //Save in Editor
                    EditorUtility.SetDirty(itemToolManager);
                    Undo.RecordObject(itemToolManager, "removeBehavior");
                    SaveSlotTypeBehaviorManager();
                }
            }
            else GUILayout.Label("Only in Pause Mode");
           
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SaveSlotTypeBehaviorManager()
    {
        EditorUtility.SetDirty(SlotTypeBehaviorManagerScript.itemTypeManager);
        Undo.RecordObject(SlotTypeBehaviorManagerScript.itemTypeManager, "removeBehavior");
    }
}