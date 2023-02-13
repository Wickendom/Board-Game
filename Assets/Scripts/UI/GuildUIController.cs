using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject questUIGO, characterSelectionUIGO;

    private static GameObject _questUIGO, _characterSelectionUIGO;

    private static bool questUIOpen = false, characterSelectionOpen = true;

    private void Start()
    {
        _questUIGO = questUIGO;
        _characterSelectionUIGO = characterSelectionUIGO;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(questUIOpen)
            {
                ToggleQuestUI();
            }
        }
    }

    public static void ToggleQuestUI()
    {
        if(questUIOpen)
        {
            DisableQuestUI();
        }
        else
        {
            EnableQuestUI();
        }
    }

    public static void ToggleCharacterSelectionUI()
    {
        if (characterSelectionOpen)
        {
            DisableCharacterCreationUI();
        }
        else
        {
            EnableCharacterCreationUI();
        }
    }

    private static void EnableCharacterCreationUI()
    {
        _characterSelectionUIGO.SetActive(true);
        characterSelectionOpen = true;
    }

    private static void EnableQuestUI()
    {
        _questUIGO.SetActive(true);
        questUIOpen = true;
    }

    private static void DisableCharacterCreationUI()
    {
        _characterSelectionUIGO.SetActive(false);
        characterSelectionOpen = false;
    }

    private static void DisableQuestUI()
    {
        _questUIGO.SetActive(false);
        questUIOpen = false;
    }
}
