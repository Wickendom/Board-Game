using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillManagerUI : MonoBehaviour
{
    public static SkillManagerUI Instance;

    public Button[] skillIncreaseButtons;
    public TextMeshProUGUI[] statLevelUIs;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void InitialiseUI(SkillManager skillManager)
    {
        skillIncreaseButtons[0].onClick.AddListener(skillManager.AddStatToVitality);
        skillIncreaseButtons[1].onClick.AddListener(skillManager.AddStatToStrength);
        skillIncreaseButtons[2].onClick.AddListener(skillManager.AddStatToDexterity);
        skillIncreaseButtons[3].onClick.AddListener(skillManager.AddStatToIntelligence);
        skillIncreaseButtons[4].onClick.AddListener(skillManager.AddStatToCombatEffectiveness);
        skillIncreaseButtons[5].onClick.AddListener(skillManager.AddStatToPerception);
        skillIncreaseButtons[6].onClick.AddListener(skillManager.AddStatToSpeed);
    }


    public void UpdateStatUIs(int[] statLevels)
    {
        for (int i = 0; i < statLevelUIs.Length; i++)
        {
            statLevelUIs[i].text = statLevels[i].ToString();
        }
    }
}
