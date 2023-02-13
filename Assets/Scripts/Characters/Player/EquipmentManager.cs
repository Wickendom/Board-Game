using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static InventoryCell headArmourSlot;
    public InventoryCell _headArmourSlot;
    public static InventoryCell torsoArmourSlot;
    public InventoryCell _torsoArmourSlot;
    public static InventoryCell beltArmourSlot;
    public InventoryCell _beltArmourSlot;
    public static InventoryCell legsArmourSlot;
    public InventoryCell _legsArmourSlot;
    public static InventoryCell feetArmourSlot;
    public InventoryCell _feetArmourSlot;

    public static InventoryCell weaponSlot;
    public InventoryCell _weaponSlot;

    private void Start()
    {
        headArmourSlot = _headArmourSlot;
        torsoArmourSlot = _torsoArmourSlot;
        beltArmourSlot = _beltArmourSlot;
        legsArmourSlot = _legsArmourSlot;
        feetArmourSlot = _feetArmourSlot;

        weaponSlot = _weaponSlot;

        if (PlayerDataManager.Instance.GetPlayerEquipmentData() != null)
        {
            InitialiseEquipmentFromData(PlayerDataManager.Instance.GetPlayerEquipmentData());
        }
    }

    private void InitialiseEquipmentFromData(PlayerEquipmentData equipmentData)
    {
        weaponSlot.item = equipmentData.weapon;

        headArmourSlot.item = equipmentData.headArmour;
        torsoArmourSlot.item = equipmentData.torsoArmour;
        beltArmourSlot.item = equipmentData.beltArmour;
        legsArmourSlot.item = equipmentData.legsArmour;
        feetArmourSlot.item = equipmentData.feetArmour;

        EquipWeapon(equipmentData.weapon);
        UpdatePlayersAC();
    }

    public static void EquipArmourPiece(Armour armour)
    {
        switch(armour.data.armourSlot)
        {
            case Armour.ArmourSlot.Head:
                {
                    headArmourSlot.SetItem(armour,1);
                    break;
                }
            case Armour.ArmourSlot.Torso:
                {
                    torsoArmourSlot.SetItem(armour,1);
                    break;
                }
            case Armour.ArmourSlot.Belt:
                {
                    beltArmourSlot.SetItem(armour,1);
                    break;
                }
            case Armour.ArmourSlot.Legs:
                {
                    legsArmourSlot.SetItem(armour,1);
                    break;
                }
            case Armour.ArmourSlot.Feet:
                {
                    feetArmourSlot.AddItem(armour);
                    break;
                }
        }

        UpdatePlayersAC();
    }

    private static void UpdatePlayersAC()
    {
        int ACTotal = 0;
        List<Armour> armours = new List<Armour>();
        armours.Add(headArmourSlot.item as Armour);
        armours.Add(torsoArmourSlot.item as Armour);
        armours.Add(beltArmourSlot.item as Armour);
        armours.Add(legsArmourSlot.item as Armour);
        armours.Add(feetArmourSlot.item as Armour);

        for (int i = 0; i < armours.Count; i++)
        {
            ACTotal += armours[i].data.armourClassModifier;
        }

        GameManager.localPlayer.SetAC(ACTotal);
    }

    public static void EquipWeapon(Weapon weapon)
    {
        GameManager.localPlayer.SetWeapon(weapon);
    }

    public void OnEnable()
    {
        SavingManager.savePlayer += SaveData;
    }

    public void OnDisable()
    {
        SavingManager.savePlayer -= SaveData;
    }

    private void SaveData()
    {
        Debug.Log("Player Equipment Saved");
        PlayerDataManager.Instance.SavePlayerEquipmentData();
    }
}
