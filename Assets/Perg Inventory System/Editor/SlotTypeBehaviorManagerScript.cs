using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "PergPanel/Item Type Manager")]
public class SlotTypeBehaviorManagerScript : ScriptableObject
{
    public static SlotTypeBehaviorManagerScript itemTypeManager;
    public List<SlotTypeBehavior> behaviors = new List<SlotTypeBehavior>();
    public SlotTypeBehavior slotTypeBehavior = new SlotTypeBehavior();

    private void OnEnable()
    {
        Debug.Log("Saveler loadlandı.");
        //LoadCreateBehaviours();
    }

    private void OnValidate()
    {
        itemTypeManager = this;
    }

    public bool CheckBehavior(SlotTypeBehavior slotTypeBehavior)
    {
        foreach (SlotTypeBehavior behavior in behaviors)
        {
            if (slotTypeBehavior.fromSlotType == behavior.fromSlotType && slotTypeBehavior.toSlotType == behavior.toSlotType)
            {
                Debug.LogError("This slot type behavior already exists.: FROM " + behavior.fromSlotType + " TO " + behavior.toSlotType);
                return false;
            }
        }
        return true;
    }
    /*public void SaveSlotTypeBehaviorList()
    {
        PlayerPrefs.SetInt("PERGbehaviorCount", behaviors.Count);
        for (int i = 0; i < behaviors.Count; i++)
        {
            PlayerPrefs.SetInt("PERG" + i + "fromSlotType", (int)behaviors[i].fromSlotType);
            PlayerPrefs.SetInt("PERG" + i + "toSlotType", (int)behaviors[i].toSlotType);
        }
    }*/
    /*private List<SlotTypeBehavior> LoadSlotTypeBehaviorList()
    {
        List<SlotTypeBehavior> loadedList = new List<SlotTypeBehavior>();
        int behaviourCount = PlayerPrefs.GetInt("PERGbehaviorCount");
        for (int i = 0; i < behaviourCount; i++)
        {
            if (PlayerPrefs.HasKey("PERG" + i + "fromSlotType"))
            {
                int fromSlotType = PlayerPrefs.GetInt("PERG" + i + "fromSlotType");
                int toSlotType = PlayerPrefs.GetInt("PERG" + i + "toSlotType");

                loadedList.Add(new SlotTypeBehavior((SlotType)fromSlotType, (SlotType)toSlotType));
            }
        }

        return loadedList;
    }*/
    /*public void RemoveFromBehaviourList(int index)
    {
        int behaviourCount = PlayerPrefs.GetInt("PERGbehaviorCount");
        if (behaviourCount <= 1)
        {
            PlayerPrefs.DeleteKey("PERGbehaviorCount");
        }
        else if (behaviourCount > 1)
        {
            behaviourCount--;
            PlayerPrefs.SetInt("PERGbehaviorCount", behaviourCount);
        }

        PlayerPrefs.DeleteKey("PERG" + index + "fromSlotType");
        PlayerPrefs.DeleteKey("PERG" + index + "toSlotType");
    }*/
    /*private void LoadCreateBehaviours()
    {
        behaviors.Clear();
        behaviors = LoadSlotTypeBehaviorList();
    }*/
    [MenuItem("PergPanel/Clear Behaviour List")]
    static void ClearBehaviourList()
    {
        itemTypeManager.behaviors.Clear();
        Debug.Log("[PERG] Behaviour list has been deleted.");
    }
    /*[MenuItem("PergPanel/Clear Behaviour Saves")]
    static void ClearBehaviourSaves()
    {
        PlayerPrefs.DeleteKey("PERGbehaviorCount");

        for (int i = 0; i < itemTypeManager.behaviors.Count; i++)
        {
            PlayerPrefs.DeleteKey("PERG" + i + "fromSlotType");
            PlayerPrefs.DeleteKey("PERG" + i + "toSlotType");
        }

        Debug.Log("[PERG] All Behaviour saves were deleted.");
    }*/
}