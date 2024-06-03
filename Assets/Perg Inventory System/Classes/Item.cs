using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemId;
    public int iconId;
    public int slotId;
    public string itemName;
    public string itemDesc;
    public int itemValue;
    public int itemMaxStack;
    public Sprite itemImage;
    public GameObject itemModel;
    public ItemType itemType;

    public Item()
    {

    }

    public Item(int itemId, int iconId, int slotId, string itemName, string itemDesc, int itemValue, int itemMaxStack, Sprite itemImage, GameObject itemModel, ItemType itemType)
    {
        this.itemId = itemId;
        this.iconId = iconId;
        this.slotId = slotId;
        this.itemName = itemName;
        this.itemDesc = itemDesc;
        this.itemValue = itemValue;
        this.itemMaxStack = itemMaxStack;
        this.itemImage = itemImage;
        this.itemModel = itemModel;
        this.itemType = itemType;
    }
    public Item NewItem(Item item)
    {
        return new Item(item.itemId, item.iconId, item.slotId, item.itemName, item.itemDesc, item.itemValue, item.itemMaxStack, item.itemImage, item.itemModel, item.itemType);
    }
}
