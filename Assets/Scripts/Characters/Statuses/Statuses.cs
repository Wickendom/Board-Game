using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "Status", menuName = "Data's/Statuses/BasicStatus")]
    public class Status : ScriptableObject
    {
        public enum StatusEffector { Action,MovementModifier,MovementValue,Skill,Turn,AttackHit,AttackDamage,EndOfTurnEffect,Initiative}
        public StatusEffector statusEffector;

        [Tooltip("This is the amount of turns it takes to remove this effect")]
        public int duration;//duration in turns the status lasts for

        [Tooltip("This identifies if the result is supposed to be negative e.g. a D4 roll of a 2 will be -2")]
        public bool negative;
        public int modifier;
        public DieType dieModifier;

        public int intValue;

        public Character unit;

        public virtual void OnApply()
        {

        }

        public virtual void OnUseAction()
        {

        }

        public virtual void OnUseSkill()
        {

        }

        public virtual void OnAttack()
        {

        }

        public virtual void OnEndTurn()
        {
            duration--;
        }
    }

    public class KnockedDown : Status
    {


        public override void OnApply()
        {

        }

        public override void OnEndTurn()
        {

        }
    }
    public class Slowed : Status
    {
        public override void OnApply()
        {

        }
    }
    public class Stunned : Status
    {
        public override void OnApply()
        {
            
        }
    }
    public class Frozen : Status
    {
        public override void OnApply()
        {

        }
    }
    public class Blinded : Status
    {
        public override void OnAttack()
        {
            
        }
    }
    public class Silenced : Status
    {
        public override void OnUseSkill()
        {
            
        }
    }
    public class DOT : Status
    {
        public override void OnEndTurn()
        {
            
        }
    }

    public class Feared: Status
    {
        public override void OnApply()
        {
            
        }
    }

    public class Taunted : Status
    {
        public override void OnApply()
        {

        }
    }

    public class Regen : Status
    {
        public override void OnEndTurn()
        {
            
        }
    }
    public class DamageProtection : Status
    {
        public override void OnApply()
        {
            
        }
    }
    public class Hasted : Status // increases initiative
    {
        public override void OnApply()
        {
            
        }
    }
    public class Blessed : Status
    {
        public override void OnApply()
        {
            
        }
    }
}