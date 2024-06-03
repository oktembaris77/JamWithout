using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    none,
    Weapon,
    Armor,
    Potion,
    Piece,
    Spell,
    Buff,
    All
}

public enum SlotType
{
    none,
    Inventory,
    SkillPage,
    SkillBar,
    Chest,
    Interactive,
    Crafted,
	LeftPanel,
	RightPanel
}

public enum PanelType
{
    none,
    Inventory,
    SkillPage,
    SkillBar,
    Chest,
    Interactive,
    Crafted,
    LeftPanel,
    RightPanel
}

public enum OtherSlotType
{
    none,
    trash,
    staff,
    buff,
    craft
}
