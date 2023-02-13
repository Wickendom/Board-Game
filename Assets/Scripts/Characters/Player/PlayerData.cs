using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public PlayerClass playerClass;

    public int playerID;

    public Stat Vitality;
    public Stat CombatEffectiveness;
    public Stat Strength;
    public Stat Dexterity;
    public Stat Intelligence;
    public Stat Speed;
    public Stat Perception;

    public int level;
    public int exp;
    public int skillPoints;

    public List<SkillData> knownSkills;

    public PlayerData(PlayerClass playerClass, int playerID)
    {
        this.playerClass = playerClass;

        this.playerID = playerID;

        Vitality = new Stat(); 
        CombatEffectiveness = new Stat(); 
        Strength = new Stat(); 
        Dexterity = new Stat(); 
        Intelligence = new Stat(); 
        Speed = new Stat(); 
        Perception = new Stat();

        Stat[] statsTemp = new Stat[] { Vitality, CombatEffectiveness, Strength, Dexterity, Intelligence, Speed, Perception };
        List<StatTypeData> statTypes = Lists.GetAllStatTypeDatas();

        for (int i = 0; i < statsTemp.Length; i++)
        {
            statsTemp[i].statTypeData = statTypes[i];
            //statsTemp[i].Initialise();
        }

        Vitality.SetStatLevel(playerClass.baseVitalityLevel);
        CombatEffectiveness.SetStatLevel(playerClass.baseCombatEffectivenessLevel);
        Strength.SetStatLevel(playerClass.baseStrengthLevel);
        Dexterity.SetStatLevel(playerClass.baseDexterityLevel);
        Intelligence.SetStatLevel(playerClass.baseIntelligenceLevel);
        Speed.SetStatLevel(playerClass.baseSpeedLevel);
        Perception.SetStatLevel(playerClass.basePerceptionLevel);

        level = 1;
        exp = 0;
        skillPoints = 0;

        knownSkills = new List<SkillData>();
        knownSkills.Add(playerClass.skillsAbleToLearn[0].skills[0]);

        //Temporarily add all skills the class can learn to known skills. this will need removing at a later date

        for (int i = 1; i < playerClass.skillsAbleToLearn.Count; i++)
        {
            for (int j = 0; j < playerClass.skillsAbleToLearn[i].skills.Length; j++)
            {
                if (playerClass.skillsAbleToLearn[i].skills[j] != null)
                    knownSkills.Add(playerClass.skillsAbleToLearn[i].skills[j]);
            }
        }
    }
}

public class PlayerEquipmentData
{
    public Weapon weapon;

    public Armour headArmour;
    public Armour torsoArmour;
    public Armour beltArmour;
    public Armour legsArmour;
    public Armour feetArmour;

    public PlayerEquipmentData()
    {
        weapon = EquipmentManager.weaponSlot.item as Weapon;

        headArmour = EquipmentManager.headArmourSlot.item as Armour;
        torsoArmour = EquipmentManager.torsoArmourSlot.item as Armour;
        beltArmour = EquipmentManager.beltArmourSlot.item as Armour;
        legsArmour = EquipmentManager.legsArmourSlot.item as Armour;
        feetArmour = EquipmentManager.feetArmourSlot.item as Armour;
    }
}
