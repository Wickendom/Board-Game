using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

[CreateAssetMenu(fileName = "Stat Type Data", menuName = "Data's/Characters/Stat Type Data")]
public class StatTypeData : ScriptableObject
{
    public List<int> levelRequirement = new List<int>();
    public List<DieType> statDice = new List<DieType>();
    public List<int> statModifier = new List<int>();

    public void AddStatLevel()
    {
        levelRequirement.Add(1);
        statDice.Add(DieType.D4);
        statModifier.Add(0);
    }

    public void RemoveStatLevel(int index)
    {
        levelRequirement.RemoveAt(index);
        statDice.RemoveAt(index);
        statModifier.RemoveAt(index);
    }
}