using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private void Awake()
    {
        instance = this;
  /*      if (FindObjectOfType<SaveSystem>() && FindObjectOfType<SaveSystem>().gameObject != gameObject)
            Destroy(gameObject);*/

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SaveSkillBarItems()
    {
        if (ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items.Count > 0)
        {
            for (int i = 0; i < ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items.Count; i++)
            {
                for (int k = 0; k < 3/*ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items.Count*/; k++)
                {
                    int itemId = ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].itemId;
                    if ((ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType == ItemType.Piece || ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType == ItemType.Weapon ||
                        ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType == ItemType.Buff) 
                        && ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].slotId == k)
                    {
                        int itemValue = ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].itemValue;
                        int itemMaxStack = ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack;

                        if (PlayerPrefs.GetInt("slotEmpty" + itemId) == 0) //Slot boþ
                        {
                            PlayerPrefs.SetInt("itemValue" + itemId, ItemDatabaseManager.instance.panels[0].panelObject.GetComponent<PergPanel>().items[i].itemValue);
                            PlayerPrefs.SetInt("slotEmpty" + itemId, 1);
                            PlayerPrefs.SetInt("itemId" + itemId, itemId);
                        }
                        else //Slot boþ deðil. Ýtem value ye ekleme yap ve-veya yeni slota ekle
                        {
                            PlayerPrefs.SetInt("itemValue" + itemId, PlayerPrefs.GetInt("itemValue" + itemId) + itemValue);
                        }
                    }
                }
            }
        }
    }
    public void LoadPermaChest(bool permaChestLoad = false, bool permaBuffChestLoad = false, bool permaStaffLoad = false)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            for (int l = 0; l < ItemDatabaseManager.instance.ItemDatabaseList.Count; l++)
            {
                int itemId = ItemDatabaseManager.instance.ItemDatabaseList.ElementAt(l).Key;

                if (permaChestLoad && PlayerPrefs.GetInt("slotEmpty" + itemId) == 1)
                {
                    /*
                    itemId
                    itemValue
                    slotEmpty
                    */

                    int fullStackCount = 0;
                    int endStackSound = 0;
                    if (PlayerPrefs.GetInt("itemValue" + itemId) > ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack)
                    {
                        fullStackCount = PlayerPrefs.GetInt("itemValue" + itemId) / ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack;
                        endStackSound = PlayerPrefs.GetInt("itemValue" + itemId) - ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack * fullStackCount;

                        for (int i = 0; i < fullStackCount; i++)
                        {
                            ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().AddItem(itemId, ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack);
                            ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                        }
                        if (endStackSound > 0)
                        {
                            ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().AddItem(itemId, endStackSound);
                            ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                        }
                    }
                    else
                    {
                        ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().AddItem(itemId, PlayerPrefs.GetInt("itemValue" + itemId));
                    }

                    ItemDatabaseManager.instance.panels[3].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                }
                if(permaBuffChestLoad && PlayerPrefs.GetInt("buffSlotEmpty" + itemId) == 1)
                {
                    int fullStackCount = 0;
                    int endStackSound = 0;
                    if (true || PlayerPrefs.GetInt("buffItemValue" + itemId) > /*ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack*/1)
                    {
                        fullStackCount = PlayerPrefs.GetInt("buffItemValue" + itemId) / /*ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack*/1;
                        endStackSound = PlayerPrefs.GetInt("buffItemValue" + itemId) - /*ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack*/1 * fullStackCount;

                        for (int i = 0; i < fullStackCount; i++)
                        {
                            ItemDatabaseManager.instance.panels[4].panelObject.GetComponent<PergPanel>().AddItem(itemId, /*ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack*/1, true);
                            ItemDatabaseManager.instance.panels[4].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                        }
                    }

                    ItemDatabaseManager.instance.panels[4].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                }
            }

            //Staff Slot
            if(permaStaffLoad)
            {
                if(PlayerPrefs.GetInt("staffItemId") == 0)
                {
                    PlayerPrefs.SetInt("staffItemId", 14);
                }
                Item staffItem = new Item(PlayerPrefs.GetInt("staffItemId"),
                                                    ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].iconId,
                                                    0, ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].itemName,
                                                    ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].itemDesc, 0, ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].itemMaxStack,
                                                    ItemDatabaseManager.instance.sprites[ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].iconId],
                                                    Resources.Load<GameObject>("GameObjects/" + 0), ItemDatabaseManager.instance.ItemDatabaseList[PlayerPrefs.GetInt("staffItemId")].itemType);
            }
        }
        else
        {
            Debug.LogError("Kalýcý envanter sadece ilk scenede loadlanabilir.");
        }
    }
    //DecreaseItem(int itemId, int decreaseValue)
    public void DecreaseItemPFSave(int itemId, int decreaseValue)
    {
        if (PlayerPrefs.GetInt("itemValue" + itemId) > decreaseValue)
        {
            PlayerPrefs.SetInt("itemValue" + itemId, PlayerPrefs.GetInt("itemValue" + itemId) - decreaseValue);
        }
        else
        {
            PlayerPrefs.DeleteKey("itemValue" + itemId);
            PlayerPrefs.DeleteKey("slotEmpty" + itemId);
            PlayerPrefs.DeleteKey("itemId" + itemId);
        }
    }
}