using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    public SkillData skillData;

    public void SetSkillData(SkillData skillData, bool setAbilityName)
    {
        this.skillData = skillData;

        Button button = GetComponent<Button>();

        button.onClick.AddListener(delegate { skillData.ShowAttackRange(); });

        skillData.Initialise();

        if(setAbilityName)
            GetComponentInChildren<TextMeshProUGUI>().text = skillData.skillName;
    }
}