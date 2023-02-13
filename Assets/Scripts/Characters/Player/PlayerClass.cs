using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Class", menuName = "Data's/Player Class")]
public class PlayerClass : CharacterData
{
    public string className;

    public List<SkillsMilestoneWrapper> skillsAbleToLearn = new List<SkillsMilestoneWrapper>();

    public ArmourData startingHeadArmour;
    public ArmourData startingTorsoArmour;
    public ArmourData startingBeltArmour;
    public ArmourData startingLegsArmour;
    public ArmourData startingFeetArmour;

    public WeaponData startingWeapon;

    public void AddNewSkillTier()
    {
        SkillsMilestoneWrapper wrapper = new SkillsMilestoneWrapper();
        wrapper.skills = new SkillData[3];
        skillsAbleToLearn.Add(wrapper);
    }

    public void RemoveSkillMilestone(int index)
    {
        skillsAbleToLearn.RemoveAt(index);
    }

    public int[] GetStatValues()
    {
        int[] stats = new int[7];

        stats[0] = baseVitalityLevel;
        stats[1] = baseCombatEffectivenessLevel;
        stats[2] = baseStrengthLevel;
        stats[3] = baseDexterityLevel;
        stats[4] = baseIntelligenceLevel;
        stats[5] = baseSpeedLevel;
        stats[6] = basePerceptionLevel;

        return stats;
    }
}

[System.Serializable]
public struct SkillsMilestoneWrapper
{
    public SkillData[] skills;
}
