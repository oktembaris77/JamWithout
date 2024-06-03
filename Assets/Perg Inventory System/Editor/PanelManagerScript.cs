using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "PergInventory/InventoryManager")]
public class PanelManagerScript : ScriptableObject
{
    public static PanelManagerScript panelManager;
    public GameObject panelSpawner;
    public List<PanelCreator> panels = new List<PanelCreator>();
    public bool autoPanelId = false;
    public int lastPanelId = 0;
    public PanelCreator panelCreator = new PanelCreator();

    private void OnEnable()
    {
        LoadCreatedPanels();
    }
    private void OnValidate()
    {
        panelSpawner = GameObject.Find("PergPanels");
        panelManager = this;

        if (autoPanelId)
        {
            panelCreator.panelId = lastPanelId;
        }

    }
    public bool PanelIdCheck(int panelId)
    {
        foreach (PanelCreator panelCreator in panels)
        {
            if (panelCreator.panelId == panelId)
            {
                Debug.LogError("A panel with this panel ID already exists. panelId: " + panelId);
                return false;
            }
        }
        return true;
    }
    public void SavePanelList(List<PanelCreator> panelList)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            PlayerPrefs.SetInt("PERGpanelCount", panelList.Count);
            PlayerPrefs.SetInt("PERG" + i + "panelId", panelList[i].panelId);
            PlayerPrefs.SetString("PERG" + panelList[i].panelId + "panelName", panelList[i].panelName);
            PlayerPrefs.SetInt("PERG" + panelList[i].panelId + "panelType", (int)panelList[i].panelType);
            PlayerPrefs.SetInt("PERG" + panelList[i].panelId + "slotCount", panelList[i].slotCount);
        }
    }
    private List<PanelCreator> LoadPanelList()
    {
        List<PanelCreator> loadedList = new List<PanelCreator>();

        int panelCount = PlayerPrefs.GetInt("PERGpanelCount");
        for (int i = 0; i < panelCount; i++)
        {
            if(PlayerPrefs.HasKey("PERG" + i + "panelId"))
            {
                int panelId = PlayerPrefs.GetInt("PERG" + i + "panelId");
                string panelName = PlayerPrefs.GetString("PERG" + panelId + "panelName");
                PanelType panelType = (PanelType)PlayerPrefs.GetInt("PERG" + panelId + "panelType");
                int slotCount = PlayerPrefs.GetInt("PERG" + panelId + "slotCount");

                GameObject panelObject = FindPanelObject(panelId);
                loadedList.Add(new PanelCreator(panelObject, panelId, panelName, panelType, slotCount));
            }
        }

        return loadedList;
    }
    public void RemoveFromPanelList(int panelId, int index)
    {
        int panelCount = PlayerPrefs.GetInt("PERGpanelCount");
        if (panelCount <= 1)
        {
            PlayerPrefs.DeleteKey("PERGpanelCount");
        }
        else if (panelCount > 1)
        {
            panelCount--;
            PlayerPrefs.SetInt("PERGpanelCount", panelCount);
        }

        PlayerPrefs.DeleteKey("PERG" + index + "panelId");

        PlayerPrefs.DeleteKey("PERG" + panelId + "panelName");
        PlayerPrefs.DeleteKey("PERG" + panelId + "panelType");
        PlayerPrefs.DeleteKey("PERG" + panelId + "slotCount");

    }
    private GameObject FindPanelObject(int panelId)
    {
        for (int k = 0; k < panelSpawner.transform.childCount; k++)
        {
            for (int m = 0; m < panelSpawner.transform.GetChild(k).childCount; m++)
            {
                if (panelSpawner.transform.GetChild(k).GetChild(m).GetComponent<PergPanel>().panelCreator.panelId == panelId)
                {
                    return panelSpawner.transform.GetChild(k).GetChild(m).gameObject;
                }
            }
        }
        return null;
    }
    private void LoadCreatedPanels()
    {
        panels = LoadPanelList();
        Debug.Log("[PERG] " + panels.Count + " panels have been added to the list.");
    }
    [MenuItem("PergPanel/Clear Panel List")]
    static void ClearPanelList()
    {
        panelManager.panels.Clear();
        Debug.Log("[PERG] Panel list has been deleted.");
    }
    [MenuItem("PergPanel/Clear Panel Saves")]
    static void ClearPanelSaves()
    {
        for (int i = 0; i < panelManager.panels.Count; i++)
        {
            PlayerPrefs.DeleteKey("PERGpanelCount");
            PlayerPrefs.DeleteKey("PERG" + i + "panelId");
            PlayerPrefs.DeleteKey("PERG" + panelManager.panels[i].panelId + "panelName");
            PlayerPrefs.DeleteKey("PERG" + panelManager.panels[i].panelId + "panelType");
            PlayerPrefs.DeleteKey("PERG" + panelManager.panels[i].panelId + "slotCount");
        }

        Debug.Log("[PERG] All panel saves were deleted.");
    }
    [MenuItem("PergPanel/Delete all playerprefs")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}