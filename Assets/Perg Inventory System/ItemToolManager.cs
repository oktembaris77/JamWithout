    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolManager : MonoBehaviour
{
    public static ItemToolManager instance;
    public GameObject panelSpawner;

	public Sprite defaultItemIcon;

	//Dragging Item
	public GameObject draggingItemObject;
    public bool draggingItemBool;
    public bool draggingItemDivideBool;
    public Item draggingItem;
    public GameObject draggingSlot;
    public int draggingPanelId = 0;
    [SerializeField] private Image draggingItemIcon = null;

    public bool panelSlotEntered = false;

    //Tooltip
    public GameObject tooltipObject;
    public bool toolTipbool;
    public Item tooltipItem;
    [SerializeField] private Text itemName = null;
    [SerializeField] private Image tooltipItemIcon = null;
    [SerializeField] private Text itemDesc = null;

    //Slot Type Behaviors
    public List<SlotTypeBehavior> behaviors = new List<SlotTypeBehavior>();

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TooltipControl();
        DraggingControl();
    }
    private void TooltipControl()
    {
        if(toolTipbool)
        {
            tooltipObject.transform.position = Input.mousePosition;
            itemName.text = tooltipItem.itemName;
            tooltipItemIcon.sprite = tooltipItem.itemImage;
            itemDesc.text = tooltipItem.itemDesc;
        }
    }
    private void DraggingControl()
    {
        if(draggingItemBool)
        {
            draggingItemObject.transform.position = Input.mousePosition;
            draggingItemIcon.sprite = draggingItem.itemImage;
        }
    }
    public bool CheckSlotTypeBehavior(SlotType fromSlotType, SlotType toSlotType)
    {
        foreach (SlotTypeBehavior slotTypeBehavior in behaviors)
        {
            if (fromSlotType == slotTypeBehavior.fromSlotType && toSlotType == slotTypeBehavior.toSlotType)
            {
                return true;
            }
        }
        return false;
    }
}
