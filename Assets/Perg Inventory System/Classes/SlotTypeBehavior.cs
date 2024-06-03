using System.Collections;
using UnityEngine;

[System.Serializable]
public class SlotTypeBehavior
{
    public SlotType fromSlotType;
    public SlotType toSlotType;

    public SlotTypeBehavior()
    {

    }

    public SlotTypeBehavior(SlotType fromSlotType, SlotType toSlotType)
    {
        this.fromSlotType = fromSlotType;
        this.toSlotType = toSlotType;
    }
}