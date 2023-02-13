using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    public enum DieType { D2, D3, D4, D6, D8, D12, D20 };

    public class Die
    {
        
        public DieType dieType = DieType.D6;

        public Die()
        {

        }

        public Die(DieType die)
        {
            dieType = die;
        }

        public void SetDieType(DieType dieType)
        {
            this.dieType = dieType;
        }

        public int RollDie()
        {
            int maxRollValue = 2;

            switch (dieType)
            {
                case DieType.D2:
                    {
                        maxRollValue = 2;
                        break;
                    }
                case DieType.D3:
                    {
                        maxRollValue = 3;
                        break;
                    }
                case DieType.D4:
                    {
                        maxRollValue = 4;
                        break;
                    }
                case DieType.D6:
                    {
                        maxRollValue = 6;
                        break;
                    }
                case DieType.D8:
                    {
                        maxRollValue = 8;
                        break;
                    }
                case DieType.D12:
                    {
                        maxRollValue = 12;
                        break;
                    }
                case DieType.D20:
                    {
                        maxRollValue = 20;
                        break;
                    }
            }
            return Random.Range(1, maxRollValue+1);
        }
    }

}