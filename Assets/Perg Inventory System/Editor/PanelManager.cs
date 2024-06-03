using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//new Item(0,0,"Asa Parçası","Asa oluşturmak için özel parça.",1,1,Resources.Load<GameObject>(""),ItemType.Piece)
//[CreateAssetMenu(menuName = "PergInventory/InventoryManager")]
[CustomEditor(typeof(PanelManagerScript))]
public class PanelManager : Editor
{
    public override void OnInspectorGUI()
    {
        //Called whenever the inspector is drawn for this object.
        //DrawDefaultInspector();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lastPanelId"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoPanelId"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panelCreator"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panelSpawner"), true);

        //This draws the default screen.  You don't need this if you want
        //to start from scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Panel"))
        {
            if (PanelManagerScript.panelManager.PanelIdCheck(PanelManagerScript.panelManager.panelCreator.panelId))
            {
                GameObject panelGameObject = Instantiate(Resources.Load<GameObject>("Panel"), PanelManagerScript.panelManager.panelSpawner.transform);
                FindAndAddPanelType(PanelManagerScript.panelManager.panelCreator.panelType, panelGameObject);
                PanelCreator panelCreator = new PanelCreator(panelGameObject, PanelManagerScript.panelManager.panelCreator.panelId, PanelManagerScript.panelManager.panelCreator.panelName,
                    PanelManagerScript.panelManager.panelCreator.panelType, PanelManagerScript.panelManager.panelCreator.slotCount);
                if (panelCreator.panelName == "" || panelCreator.panelName == null)
                    panelCreator.panelName = "Panel" + panelCreator.panelId;
                panelGameObject.name = panelCreator.panelName;

                panelGameObject.GetComponent<PergPanel>().panelCreator = panelCreator;

                //Add Panel
                PanelManagerScript.panelManager.panels.Add(panelCreator);
                //Save Panel
                PanelManagerScript.panelManager.SavePanelList(PanelManagerScript.panelManager.panels);

                //lastPanelId Update
                PanelManagerScript.panelManager.lastPanelId++;
                if (PanelManagerScript.panelManager.autoPanelId)
                    PanelManagerScript.panelManager.panelCreator.panelId = PanelManagerScript.panelManager.lastPanelId;

            }
        }

        //InventoryManagerScript.ims.removeInventoryWithId = GUILayout.Toggle(!InventoryManagerScript.ims.removeInventoryWithId, "With Id", "Radio");
        //InventoryManagerScript.ims.removeInventoryWithId = GUILayout.Toggle(!InventoryManagerScript.ims.removeInventoryWithId, "With Index", "Radio");

        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < PanelManagerScript.panelManager.panels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            PanelCreator pc = PanelManagerScript.panelManager.panels[i];
            EditorGUILayout.ObjectField(PanelManagerScript.panelManager.panels[i].panelObject, typeof(PanelCreator), false);
            GUILayout.Label("Id: " + PanelManagerScript.panelManager.panels[i].panelId);
            if (PanelManagerScript.panelManager.panels[i].panelObject != null)
                GUILayout.Label("Slot: " + PanelManagerScript.panelManager.panels[i].panelObject.GetComponent<PergPanel>().panelCreator.slotCount);
            if (GUILayout.Button("+ Slot"))
            {
                PanelManagerScript.panelManager.panels[i].panelObject.GetComponent<PergPanel>().panelCreator.slotCount++;
        
            }
            if (GUILayout.Button("- Slot"))
            {
                PanelManagerScript.panelManager.panels[i].panelObject.GetComponent<PergPanel>().panelCreator.slotCount--;
               
            }
            if (GUILayout.Button("Remove Panel"))
            {
                //Remove Panel
                DestroyImmediate(PanelManagerScript.panelManager.panels[i].panelObject);
                PanelManagerScript.panelManager.panels.RemoveAt(i);
                //Remove Panel Save
                PanelManagerScript.panelManager.RemoveFromPanelList(PanelManagerScript.panelManager.panels[i].panelId, i);

                //PanelId Update
                if (PanelManagerScript.panelManager.autoPanelId)
                    PanelManagerScript.panelManager.panelCreator.panelId = PanelManagerScript.panelManager.lastPanelId;
            }

            EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void FindAndAddPanelType(PanelType panelType, GameObject panelGameObject)
    {
        bool available = false;
        for (int i = 0; i < PanelManagerScript.panelManager.panelSpawner.transform.childCount; i++)
        {
            if (PanelManagerScript.panelManager.panelSpawner.transform.GetChild(i).name == panelType.ToString())
            {
                available = true;
                panelGameObject.transform.SetParent(PanelManagerScript.panelManager.panelSpawner.transform.GetChild(i));
                break;
            }
        }

        if (!available)
        {
            GameObject emptyPanel = Instantiate(Resources.Load<GameObject>("EmptyPanel"), PanelManagerScript.panelManager.panelSpawner.transform);
            emptyPanel.name = panelType.ToString();
            panelGameObject.transform.SetParent(emptyPanel.transform);
        }
    }
}