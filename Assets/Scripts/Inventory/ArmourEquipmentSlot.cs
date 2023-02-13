using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourEquipmentSlot : InventoryCell
{
    public override void AddItem(Item Item)
    {
        if(item is IEquipable)
            base.AddItem(Item);
    }
}
