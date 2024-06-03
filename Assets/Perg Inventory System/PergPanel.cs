using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PergPanel : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<PanelSlot> slots = new List<PanelSlot>();
    public PanelCreator panelCreator;
    public GameObject slotSpawner;

    public ItemDatabaseManager database;

    /*public GameObject tooltip;
    public bool draggingItemBool, draggingItemDivideBool;
    public Item draggingItem;
    public GameObject draggingItemImage;
    public bool toolTipbool;
    public Item tooltipItem;*/

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        //PanelSlotSpawner();
        //AddMe();
    }

    // Update is called once per frame
    void Update()
    {
    }
    /*
    public void UpdateSlotAndItems()
    {
        items.Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].emptySlot) continue;

            items.Add(new Item().NewItem(slots[i].item));
        }
    }
    */
    public void FillInventorySlot()
    {
        //Items listesi oldurulduktan sonra(Database vb.), Items listesindeki itemleri slotlara koy
        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log("Slotid: " + i + " " + items[i].slotId);
            slots[items[i].slotId].item = items[i];
            slots[items[i].slotId].emptySlot = false;
        }
    }
    public void PanelSlotSpawner()
    {
        for (int i = 0; i < panelCreator.slotCount; i++)
        {
            GameObject slot = Instantiate(Resources.Load<GameObject>("PanelSlot"));
            PanelSlot inventorySlot = slot.GetComponent<PanelSlot>();
            slots.Add(inventorySlot);
            slot.transform.SetParent(slotSpawner.transform);
            inventorySlot.slotId = i;
            inventorySlot.slotType = (SlotType)panelCreator.panelType;
            slot.name = "Slot " + (i+1);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="splitItem">Itemid aynı olsa da slotlara ayrı ayrı koy.</param>
    /// <returns></returns>
    public bool AddItem(int itemId, int itemValue, bool splitItem = false)
    {
        int panelId = panelCreator.panelId;
        int[] getEmpty = ItemDatabaseManager.instance.panels[panelId].panelObject.GetComponent<PergPanel>().GetEmptySlotId(itemId, splitItem); //slotId, itemValue

        if (getEmpty[1] != 0 && getEmpty[1] < ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack && !splitItem) //Slot doluysa ve yer varsa
        {
            for (int i = 0; i < ItemDatabaseManager.instance.panels[panelId].panelObject.GetComponent<PergPanel>().items.Count; i++)
            {
                if (ItemDatabaseManager.instance.panels[panelId].panelObject.GetComponent<PergPanel>().items[i].itemId == itemId)
                {
                    int panelItemValue = getEmpty[1];
                    int panelItemMaxStack = ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack;

                    if (panelItemValue < panelItemMaxStack) //Bu item içine item eklenebilir
                    {
                        //Item içine eklenebilecek miktar alınır
                        int addingItemValue = panelItemMaxStack - panelItemValue;

                        //Eğer eklencek item miktarı, eklenebiliecek miktardan azsa veya eşitse direk eklenir.
                        if (itemValue <= addingItemValue)
                        {
                            ItemDatabaseManager.instance.panels[panelId].panelObject.GetComponent<PergPanel>().items[i].itemValue += itemValue;
                        }
                        else if (itemValue > addingItemValue)
                        {
                            //Eğer eklenecek item miktarı, eklenebilecek miktardan fazla ise;
                            //Bu itemın değeri, maxStack ile eşitlenir.
                            ItemDatabaseManager.instance.panels[panelId].panelObject.GetComponent<PergPanel>().items[i].itemValue = panelItemMaxStack;

                            //(Eklenecek item miktarı - eklenebilecek miktar) değeri alınır ve yeni item slotuna koyulur.
                            int itemValueDiff = itemValue - addingItemValue;

                            Item item = new Item(itemId,
                                                     ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId,
                                                     getEmpty[0], ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemName,
                                                     ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemDesc, itemValueDiff, ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack,
                                                     ItemDatabaseManager.instance.sprites[ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId],
                                                     Resources.Load<GameObject>("GameObjects/" + 0), ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType);

                            ItemDatabaseManager.instance.FillPanel(panelId, item);
                        }
                    }
                    else
                    {
                        Item item = new Item(itemId,
                                                    ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId,
                                                    getEmpty[0], ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemName,
                                                    ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemDesc, itemValue, ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack,
                                                    ItemDatabaseManager.instance.sprites[ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId],
                                                    Resources.Load<GameObject>("GameObjects/" + 0), ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType);

                        ItemDatabaseManager.instance.FillPanel(panelId, item);
                    }
                }
            }
            return true;
        }
        else if (getEmpty[1] == 0)
        {
            Item item = new Item(itemId,
                        ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId,
                        getEmpty[0], ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemName,
                        ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemDesc, itemValue, ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemMaxStack,
                        ItemDatabaseManager.instance.sprites[ItemDatabaseManager.instance.ItemDatabaseList[itemId].iconId],
                        Resources.Load<GameObject>("GameObjects/" + 0), ItemDatabaseManager.instance.ItemDatabaseList[itemId].itemType);

            ItemDatabaseManager.instance.FillPanel(panelId, item);

            return true;
        }

        return true;
    }
    public void DecreaseItem(int itemId, int decreaseValue, int craftItemSlotId, bool spellUse = false)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if ((spellUse && !slots[i].emptySlot && slots[i].item.itemId == itemId) || (!spellUse && !slots[i].emptySlot && craftItemSlotId == i && slots[i].item.itemId == itemId))
            {
                if (slots[i].item.itemValue > decreaseValue)
                {
                    slots[i].item.itemValue -= decreaseValue;
                    break;
                }
                else if (slots[i].item.itemValue == decreaseValue)
                {
                    int itemCounts = ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items.Count;
                    for (int k = 0; k < itemCounts; k++)
                    {
                        Debug.Log("Item Id Kontrol: " + ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items[k].itemId + " -> " + itemId);
                        if ((ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items[k].itemId == itemId && spellUse) ||
                            (ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items[k].itemId == itemId &&
                            ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items[k].slotId == i &&
                            !spellUse))
                        {
                            ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().items.RemoveAt(k);
                            slots[i].item = null;
                            slots[i].emptySlot = true;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        ItemDatabaseManager.instance.panels[panelCreator.panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();
    }
    public void DeleteItemInPanelWithItemId(int itemId)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemId == itemId)
            {
                items.RemoveAt(i);
            }
        }
    }
    public void DeleteItemInPanelWithSlotId(int slotId)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].slotId == slotId)
            {
                items.RemoveAt(i);
            }
        }
    }
    public void AddMe()
    {
        if(!ItemDatabaseManager.instance.panels.TryGetValue(panelCreator.panelId, out PanelCreator v))
        {
            ItemDatabaseManager.instance.panels.Add(panelCreator.panelId, panelCreator);
            slotSpawner.GetComponent<Slots>().panelId = panelCreator.panelId;
            slotSpawner.GetComponent<Slots>().panelType = panelCreator.panelType;
        }
        else
        {
            ItemDatabaseManager.instance.panels[panelCreator.panelId] = panelCreator;
        }
    }
    public int[] GetEmptySlotId(int lootId = -1, bool splitItem = false)
    {
        int[] values = new int[2]; //Slot id, itemValue
        int itemValue = 0;
        if (!splitItem)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].emptySlot && slots[i].item.itemId == lootId && slots[i].item.itemMaxStack > slots[i].item.itemValue)
                {
                    values[0] = slots[i].slotId;
                    values[1] = slots[i].item.itemValue;

                    return values;
                }
                else if (!slots[i].emptySlot && slots[i].item.itemId == lootId && slots[i].item.itemMaxStack == slots[i].item.itemValue)
                {
                    values[1] = slots[i].item.itemValue;
                }
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].emptySlot)
            {
                values[0] = slots[i].slotId;
                values[1] = 0;
                return values;
            }
        }

        values[0] = -1;
        values[1] = itemValue;

        return values;
    }
}
