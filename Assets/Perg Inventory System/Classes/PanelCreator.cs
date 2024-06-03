using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelCreator
{
    public GameObject panelObject;
    public int panelId;
    public string panelName = "Inventory";
    public PanelType panelType;
    public int slotCount;

    public PanelCreator()
    {

    }
    public PanelCreator(GameObject panelObject, int panelId, string panelName, PanelType panelType, int slotCount)
    {
        this.panelObject = panelObject;
        this.panelId = panelId;
        this.panelName = panelName;
        this.panelType = panelType;
        this.slotCount = slotCount;
    }
}