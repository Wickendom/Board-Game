using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : Item, IEquipable
{
    public ArmourData data = new ArmourData();
    public enum ArmourSlot { Head,Torso,Belt,Legs,Feet}


    public void Equip()
    {
        throw new System.NotImplementedException();
    }
}

[CreateAssetMenu(fileName = "Armour Data", menuName = "Data's/ArmourData")]
public class ArmourData : ItemData
{
    public ArmourData()
    {
        armourClassModifier = Random.Range(0, 6);
        armourSlot = (Armour.ArmourSlot)Random.Range(0, 6);
    }

    public ArmourData(int armourClassModifier,Armour.ArmourSlot armourSlot)
    {
        this.armourClassModifier = armourClassModifier;
        this.armourSlot = armourSlot;
    }

    public readonly int armourClassModifier;
    public readonly Armour.ArmourSlot armourSlot;
}
