using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    public enum DamageType { Blunt, Sharp, Fire, Acid, Ice, Wind, Lightning }

    public struct DamageData
    {
        public int amount;
        public DamageType type;
        
        public DamageData(int amount, DamageType type)
        {
            this.amount = amount;
            this.type = type;
        }
    }
}

