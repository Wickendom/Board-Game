using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject abilityActionButtonPrefab;

    private enum AbilityUIState { BaseTier, SecondAbilityTier, SecondItemTier}
    private AbilityUIState abilityUIState = AbilityUIState.BaseTier;

    [SerializeField]
    private AbilityButton baseAbilityAttackButton;

    [SerializeField]
    private Transform[] baseTierbuttonPositions;
    [SerializeField]
    private Transform[] secondTierAbilityButtonPositions;
    [SerializeField]
    private GameObject[] baseTierActionButtons;
    [SerializeField]
    private Transform offScreenButttonPosition;

    private List<GameObject> secondTierAbilityButtons;

    private int currentAbilityHighlightedIndex = 2;

    public bool localPlayersTurn;

    private void Awake()
    {
        secondTierAbilityButtons = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialise()
    {
        baseAbilityAttackButton.SetSkillData(GameManager.localPlayer.knownSkills[0], false);

        CreateAbilityButtons();
    }

    public void CreateAbilityButtons()
    {
        SkillData[] abilities = GameManager.localPlayer.knownSkills.ToArray();

        for (int i = 1; i < abilities.Length; i++)
        {
            GameObject button = Instantiate(abilityActionButtonPrefab, transform);

            button.GetComponent<AbilityButton>().SetSkillData(abilities[i],true);

            button.name = "2nd Tier Ability Button " + (abilities[i].skillName);

            secondTierAbilityButtons.Add(button);
        }

        OrderAbilityButtons();
    }

    public void OrderAbilityButtons()
    {        
        for (int i = 0; i < secondTierAbilityButtons.Count; i++)
        {
            secondTierAbilityButtons[i].transform.position = offScreenButttonPosition.position;
        }

        secondTierAbilityButtons[secondTierAbilityButtons.Count - 1].transform.position = secondTierAbilityButtonPositions[0].position;
        secondTierAbilityButtons[0].transform.position = secondTierAbilityButtonPositions[1].position;
        secondTierAbilityButtons[1].transform.position = secondTierAbilityButtonPositions[2].position;
    }

    public void MoveAbilitiesUp()
    {
        currentAbilityHighlightedIndex--;
        if(currentAbilityHighlightedIndex < 0)
        {
            currentAbilityHighlightedIndex = secondTierAbilityButtons.Count-1;
        }

        int tempIndex = currentAbilityHighlightedIndex + 3;

        if(tempIndex > secondTierAbilityButtons.Count - 1)
        {
            int tempdiff = tempIndex - (secondTierAbilityButtons.Count - 1);
            tempIndex = -1 + tempdiff;
        }
        else if(tempIndex < 0)
        {
            tempIndex = (secondTierAbilityButtons.Count - 1) - Mathf.Abs(tempIndex);
        }

        for (int i = 4; i > 1; i--)
        {
            if(i != 0)
            {
                secondTierAbilityButtons[tempIndex].transform.position = secondTierAbilityButtonPositions[i - 2].position;
            }
            if(i == 4)
            {
                int temp = tempIndex;
                if (temp >= secondTierAbilityButtons.Count - 1)
                    temp = -1;
                secondTierAbilityButtons[temp+1].transform.position = offScreenButttonPosition.position;
            }

            tempIndex--;
            if(tempIndex < 0)
            {
                tempIndex = secondTierAbilityButtons.Count - 1;
            }
        }
    }

    public void MoveAbilitiesDown()
    {
        currentAbilityHighlightedIndex++;
        if (currentAbilityHighlightedIndex > secondTierAbilityButtons.Count - 1)
        {
            currentAbilityHighlightedIndex = 0;
        }

        int tempIndex = currentAbilityHighlightedIndex + 3;

        if (tempIndex > secondTierAbilityButtons.Count - 1)
        {
            int tempdiff = tempIndex - (secondTierAbilityButtons.Count - 1);
            tempIndex = -1 + tempdiff;
        }
        else if (tempIndex < 0)
        {
            tempIndex = (secondTierAbilityButtons.Count - 1) - Mathf.Abs(tempIndex);
        }

        for (int i = 4; i > 1; i--)
        {
            if (i != 0)
            {
                secondTierAbilityButtons[tempIndex].transform.position = secondTierAbilityButtonPositions[i - 2].position;
            }
            if (i == 4)
            {
                int temp = tempIndex;
                if (temp >= secondTierAbilityButtons.Count - 1)
                    temp = -1;
                secondTierAbilityButtons[temp + 1].transform.position = offScreenButttonPosition.position;
            }

            tempIndex++;
            if (tempIndex > secondTierAbilityButtons.Count - 1)
            {
                tempIndex = 0;
            }
        }
    }

    public void MoveToAbilitySecondTier()
    {
        if(secondTierAbilityButtons.Count > 0)
        {
            ShowSecondTierAbilityButtons();
            abilityUIState = AbilityUIState.SecondAbilityTier;
        }
    }

    public void MoveToItemSecondTier()
    {

    }

    public void MoveToBaseTier()
    {

    }

    public void ShowSecondTierAbilityButtons()
    {
        for (int i = 0; i < secondTierAbilityButtons.Count; i++)
        {
            secondTierAbilityButtons[i].SetActive(true);
        }
    }

    public void HideSecondTierAbilityButtons()
    {
        for (int i = 0; i < secondTierAbilityButtons.Count; i++)
        {
            secondTierAbilityButtons[i].SetActive(true);
        }
    }

    public void ShowSecondTierItemButtons()
    {
        for (int i = 0; i < secondTierAbilityButtons.Count; i++)
        {
            secondTierAbilityButtons[i].SetActive(true);
        }
    }

    public void HideSecondTierItemButtons()
    {
        for (int i = 0; i < secondTierAbilityButtons.Count; i++)
        {
            secondTierAbilityButtons[i].SetActive(true);
        }
    }
}
