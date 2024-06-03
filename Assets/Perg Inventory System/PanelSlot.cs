using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelSlot : MonoBehaviour,
    IDragHandler,
    IDropHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    //Slotun içindeki item imagesi no raycast olcak ve itemslotunda item yoksa tooltip görünmicek
    public SlotType slotType;
    public OtherSlotType otherSlotType;
    public int slotId = 0;
    public Item item = null;
    public bool emptySlot = true;
    public Text itemCountText;

    public Image itemImage;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Slotlara itemler eklenince 1 kere çağrılacak

        SlotActions();
    }
    private void SlotActions()
    {
        if (/*item != null*/!emptySlot)
        {
            itemImage.sprite = ItemDatabaseManager.instance.sprites[item.iconId];
            if (item.itemValue > 1)
            {
                itemCountText.text = item.itemValue.ToString();
            }
            else
            {
                itemCountText.text = "";
            }
            //emptySlot = false;
        }
        else
        {
            itemImage.sprite = ItemToolManager.instance.defaultItemIcon;
            if (itemCountText != null)
                itemCountText.text = "";
            //emptySlot = true;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button.ToString() == "Left" && !ItemToolManager.instance.draggingItemBool && otherSlotType != OtherSlotType.buff)
        {
            if (!emptySlot)
            {
                ItemToolManager.instance.draggingItemObject.SetActive(true);
                ItemToolManager.instance.draggingItemBool = true;
                ItemToolManager.instance.draggingItem = item;

                ItemToolManager.instance.draggingSlot = gameObject;
                ItemToolManager.instance.draggingPanelId = 0;

				item = null;
                emptySlot = true;
            }
        }
        else if (eventData.button.ToString() == "Right" && !ItemToolManager.instance.draggingItemBool)
        {
            if (!emptySlot && item.itemValue > 1)
            {
                int newItemVal = Mathf.FloorToInt(item.itemValue / 2);
                int newDraggingItemVal = item.itemValue - newItemVal;

                ItemToolManager.instance.draggingItemObject.SetActive(true);
                ItemToolManager.instance.draggingItemBool = true;
                ItemToolManager.instance.draggingItemDivideBool = true;

                Item newItem = new Item(item.itemId, item.iconId, item.slotId,
                        item.itemName, item.itemDesc, newDraggingItemVal,
                        item.itemMaxStack, item.itemImage, item.itemModel,
                        item.itemType);

                ItemToolManager.instance.draggingItem = newItem;

                ItemToolManager.instance.draggingSlot = gameObject;
                ItemToolManager.instance.draggingPanelId = 0;

                item.itemValue = newItemVal;

            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (ItemToolManager.instance.draggingItemBool && eventData.button.ToString() == "Left" && !ItemToolManager.instance.draggingItemDivideBool)
        {
            if (ItemToolManager.instance.CheckSlotTypeBehavior(ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().slotType, slotType) && otherSlotType != OtherSlotType.buff &&
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().otherSlotType != OtherSlotType.buff)
            {
                if (emptySlot) // Boş slota koy
                {
                    int iSlotId = slotId;

                    ItemToolManager.instance.draggingItemObject.SetActive(false);
                    ItemToolManager.instance.draggingItemBool = false;
                    item = ItemToolManager.instance.draggingItem;

                    //Slot ve items i eşitle
                    //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();

                    if (ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().slotType == slotType)
                        item.slotId = iSlotId;
                    else if (slotType != SlotType.Interactive)
                    {
                        Debug.Log("Panel Id: " + ItemToolManager.instance.draggingSlot.transform.parent.GetComponent<Slots>().panelId);
                        ItemDatabaseManager.instance.panels[ItemToolManager.instance.draggingSlot.transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().DeleteItemInPanelWithSlotId(item.slotId);

                        //Yorum yapıldı
                        Item newItem = new Item(item.itemId, item.iconId, slotId, item.itemName, item.itemDesc, item.itemValue, item.itemMaxStack, item.itemImage, item.itemModel, item.itemType);
                        ItemDatabaseManager.instance.FillPanel(transform.parent.GetComponent<Slots>().panelId, newItem);
                        ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();

                    }

                    emptySlot = false;

                    if (slotType != SlotType.Interactive)
                    {
                        ItemToolManager.instance.draggingItem = null;
                    }
                    else if (otherSlotType == OtherSlotType.craft)
                    {
                        //Geldiği slotta da kalsın
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                    }
                }
                else if (slotType != SlotType.Interactive && ItemToolManager.instance.draggingItem.itemId != item.itemId &&
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().slotType == slotType) // Yer değiştir    
                {
                    ItemToolManager.instance.draggingItemObject.SetActive(false);
                    ItemToolManager.instance.draggingItemBool = false;
                    int iSlot1 = item.slotId; //Bırakılan slot id
                    int iSlot2 = ItemToolManager.instance.draggingItem.slotId; //Alınan slot id
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = item;
                    item = ItemToolManager.instance.draggingItem;

                    //Slot ve items i eşitle
                    //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();


                    if (slotType == SlotType.Inventory)
                    {
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item.slotId = iSlot2;
                        item.slotId = iSlot1;
                    }

                    emptySlot = false;
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                    ItemToolManager.instance.draggingItem = null;

                }
                else if (slotType != SlotType.Interactive && ItemToolManager.instance.draggingItem.itemId == item.itemId)
                {
                    if (item.itemValue + ItemToolManager.instance.draggingItem.itemValue <= item.itemMaxStack)
                    {

                        //Panel aynı mı
                        //Panel aynı ise
                        int thisPanelId = transform.parent.GetComponent<Slots>().panelId;
                        int otherPanelId = ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().transform.parent.GetComponent<Slots>().panelId;
                        if (thisPanelId == otherPanelId || true )
                        {
                            item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
                            ItemDatabaseManager.instance.panels[otherPanelId].panelObject.GetComponent<PergPanel>().DeleteItemInPanelWithSlotId(ItemToolManager.instance.draggingItem.slotId);
                        }
                        //Panel aynı değil ise
                        else
                        {
                            //Yorum yapıldı  GEÇİCİ
                            /*	Item newItem = new Item(item.itemId, item.iconId, slotId, item.itemName, item.itemDesc, item.itemValue, item.itemMaxStack, item.itemImage, item.itemModel, item.itemType);
								ItemDatabaseManager.instance.FillPanel(transform.parent.GetComponent<Slots>().panelId, newItem);
								ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();
							*/

                        }

                    }
                    else
                    {
                        int newDragItemVal = item.itemMaxStack - item.itemValue;
                        item.itemValue = item.itemMaxStack;
                        ItemToolManager.instance.draggingItem.itemValue -= newDragItemVal;

                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;

                        //Slot ve items i eşitle
                        //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();
                    }

                    ItemToolManager.instance.draggingItemObject.SetActive(false);
                    ItemToolManager.instance.draggingItemBool = false;
                    ItemToolManager.instance.draggingItem = null;
                }
                else
                {
                    ItemToolManager.instance.draggingItemObject.SetActive(false);
                    ItemToolManager.instance.draggingItemBool = false;
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;
                    ItemToolManager.instance.draggingItem = null;

                    //Slot ve items i eşitle
                    //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();
                }
            }
            else if (otherSlotType == OtherSlotType.trash)
            {
                Debug.LogError("SaveSystem -> panels dictionary keyleri sabit değer!");
                Debug.Log("Item Deleted: " + ItemToolManager.instance.draggingItem.itemId + " Value: " + ItemToolManager.instance.draggingItem.itemValue);

                SaveSystem.instance.DecreaseItemPFSave(ItemToolManager.instance.draggingItem.itemId, ItemToolManager.instance.draggingItem.itemValue);

                ItemToolManager.instance.draggingItemObject.SetActive(false);
                ItemToolManager.instance.draggingItemBool = false;
                ItemToolManager.instance.draggingItem = null;

                //Slot ve items i eşitle
                //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();
            }
            else
            {
                ItemToolManager.instance.draggingItemObject.SetActive(false);
                ItemToolManager.instance.draggingItemBool = false;
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;
                ItemToolManager.instance.draggingItem = null;

                //Slot ve items i eşitle
                //ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().UpdateSlotAndItems();
            }
        }
        else if (eventData.button.ToString() == "Right" && ItemToolManager.instance.draggingItemBool && ItemToolManager.instance.draggingItemDivideBool)
        {
            if (ItemToolManager.instance.CheckSlotTypeBehavior(ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().slotType, slotType))
            {
                if (emptySlot)
                {
                    int iSlotId = slotId;

                    ItemToolManager.instance.draggingItemObject.SetActive(false);
                    ItemToolManager.instance.draggingItemBool = false;
                    ItemToolManager.instance.draggingItemDivideBool = false;
                    //item = ItemToolManager.instance.draggingItem;

                    Item newItem = new Item(ItemToolManager.instance.draggingItem.itemId, ItemToolManager.instance.draggingItem.iconId, iSlotId,
                        ItemToolManager.instance.draggingItem.itemName, ItemToolManager.instance.draggingItem.itemDesc, ItemToolManager.instance.draggingItem.itemValue,
                        ItemToolManager.instance.draggingItem.itemMaxStack, ItemToolManager.instance.draggingItem.itemImage, ItemToolManager.instance.draggingItem.itemModel,
                        ItemToolManager.instance.draggingItem.itemType);



                    // Yorum yapıldı
                    ItemDatabaseManager.instance.FillPanel(transform.parent.GetComponent<Slots>().panelId, newItem);
                    ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();


                    /*
                    if (ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().slotType == slotType)
                    {
                        item = new Item(ItemToolManager.instance.draggingItem.itemId, ItemToolManager.instance.draggingItem.iconId, iSlotId,
                        ItemToolManager.instance.draggingItem.itemName, ItemToolManager.instance.draggingItem.itemDesc, ItemToolManager.instance.draggingItem.itemValue,
                        ItemToolManager.instance.draggingItem.itemMaxStack, ItemToolManager.instance.draggingItem.itemImage, ItemToolManager.instance.draggingItem.itemModel,
                        ItemToolManager.instance.draggingItem.itemType);
                    }
                    else if (slotType != SlotType.Interactive)
                    {
                        Item newItem = new Item(ItemToolManager.instance.draggingItem.itemId, ItemToolManager.instance.draggingItem.iconId, iSlotId,
                        ItemToolManager.instance.draggingItem.itemName, ItemToolManager.instance.draggingItem.itemDesc, ItemToolManager.instance.draggingItem.itemValue,
                        ItemToolManager.instance.draggingItem.itemMaxStack, ItemToolManager.instance.draggingItem.itemImage, ItemToolManager.instance.draggingItem.itemModel,
                        ItemToolManager.instance.draggingItem.itemType);

                        ItemDatabaseManager.instance.FillPanel(transform.parent.GetComponent<Slots>().panelId, newItem);
                        ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                    }*/

                    emptySlot = false;

                    if (slotType != SlotType.Interactive)
                    {
                        ItemToolManager.instance.draggingItem = null;
                    }
                    else
                    {
                        //Geldiği slotta da kalsın
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;
                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                    }

                }
                else if (slotType != SlotType.Interactive && ItemToolManager.instance.draggingItem.itemId == item.itemId) // Bölmede yer değiştirme olmayacak. İtem birleştirme olacak
                {
                    ItemToolManager.instance.draggingItemDivideBool = false;

                    if (item.itemValue + ItemToolManager.instance.draggingItem.itemValue <= item.itemMaxStack)
                    {
                        item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
                        Debug.Log("right drag item. Fazlası konmaya çalışılmadı.");
                    }
                    else
                    {
                        /*
                         ItemDatabaseManager.instance.panels[ItemToolManager.instance.draggingSlot.transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().DeleteItemInPanelWithSlotId(item.slotId);

                        //Yorum yapıldı
                        Item newItem = new Item(item.itemId, item.iconId, slotId, item.itemName, item.itemDesc, item.itemValue, item.itemMaxStack, item.itemImage, item.itemModel, item.itemType);
                        ItemDatabaseManager.instance.FillPanel(transform.parent.GetComponent<Slots>().panelId, newItem);
                        ItemDatabaseManager.instance.panels[transform.parent.GetComponent<Slots>().panelId].panelObject.GetComponent<PergPanel>().FillInventorySlot();
                        */

                        Debug.Log("right drag item. Fazlası konmaya çalışıldı ve dragging olan a fazlası iade edildi.");

                        int newDragItemVal = item.itemMaxStack - item.itemValue;
                        item.itemValue = item.itemMaxStack;
                        ItemToolManager.instance.draggingItem.itemValue -= newDragItemVal;

                        ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
                    }

                    //TODO: Yukarıda left ile item bileştirme yapılacak!


                }
                else if (slotType != SlotType.Interactive && ItemToolManager.instance.draggingItem.itemId != item.itemId)
                {
                    ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
                }
                else
                {

                }
            }
            else
            {
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
            }

            ItemToolManager.instance.draggingItemObject.SetActive(false);
            ItemToolManager.instance.draggingItemBool = false;
            ItemToolManager.instance.draggingItemDivideBool = false;
            ItemToolManager.instance.draggingItem = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!emptySlot)
        {
            ItemToolManager.instance.tooltipItem = item;
            ItemToolManager.instance.toolTipbool = true;
            ItemToolManager.instance.tooltipObject.SetActive(true);

            if(Input.mousePosition.y <= 840.0f)
            {
                ItemToolManager.instance.tooltipObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            }
            else
            {
                ItemToolManager.instance.tooltipObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            }
        }
        ItemToolManager.instance.panelSlotEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemToolManager.instance.tooltipItem = null;
        ItemToolManager.instance.toolTipbool = false;
        ItemToolManager.instance.tooltipObject.SetActive(false);
        ItemToolManager.instance.panelSlotEntered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button.ToString() == "Left")
        {
            if (ItemToolManager.instance.draggingItemBool && !ItemToolManager.instance.draggingItemDivideBool && !ItemToolManager.instance.panelSlotEntered)
            {
                ItemToolManager.instance.draggingItemObject.SetActive(false);
                ItemToolManager.instance.draggingItemBool = false;
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().emptySlot = false;
                ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item = ItemToolManager.instance.draggingItem;
                ItemToolManager.instance.draggingItem = null;
            }
        }
		else if (eventData.button.ToString() == "Right")
		{
			if (ItemToolManager.instance.draggingItemBool && ItemToolManager.instance.draggingItemDivideBool && !ItemToolManager.instance.panelSlotEntered)
			{
				ItemToolManager.instance.draggingSlot.GetComponent<PanelSlot>().item.itemValue += ItemToolManager.instance.draggingItem.itemValue;
				ItemToolManager.instance.draggingItemObject.SetActive(false);
				ItemToolManager.instance.draggingItemBool = false;
				ItemToolManager.instance.draggingItemDivideBool = false;
				ItemToolManager.instance.draggingItem = null;
			}
		}
	}
}