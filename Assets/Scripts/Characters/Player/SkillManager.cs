using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    BoardGame.Player player;

    public SkillManagerUI skillManagerUI;

    public SkillManager(BoardGame.Player player)
    {
        this.player = player;
    }

    public void InitialiseUI(int[] statLevels)
    {
        skillManagerUI.InitialiseUI(this);
        skillManagerUI.UpdateStatUIs(statLevels);
    }

    private void UpdateStatUI()
    {
        skillManagerUI.UpdateStatUIs(player.GetStatLevels());
    }

    private bool CheckPlayerHasSkillPoint()
    {
        if(player.currSkillPoints > 0)
        {
            player.currSkillPoints--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddStatToVitality()
    {
        if(CheckPlayerHasSkillPoint())
        {
            player.Vitality.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToCombatEffectiveness()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.CombatEffectiveness.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToStrength()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.Strength.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToDexterity()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.Dexterity.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToIntelligence()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.Intelligence.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToSpeed()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.Speed.IncreaseStatLevel();
            UpdateStatUI();
        }
    }

    public void AddStatToPerception()
    {
        if (CheckPlayerHasSkillPoint())
        {
            player.Perception.IncreaseStatLevel();
            UpdateStatUI();
        }
    }
}
