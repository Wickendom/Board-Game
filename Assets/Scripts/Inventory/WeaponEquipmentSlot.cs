using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipmentSlot : InventoryCell
{
    public override void AddItem(Item Item)
    {
        if (item is Weapon)
            EquipmentManager.EquipWeapon(item as Weapon);
    }
}
