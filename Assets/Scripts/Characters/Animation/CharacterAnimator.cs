using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Character unit;

    [SerializeField]
    private Animator anim;

    public void Initialise(Character character)
    {
        unit = character;

        if (GetComponentsInChildren<Animator>(true).Length > 0)
        {
            Debug.Log("Animator found");
            anim = GetComponentInChildren<Animator>();

            unit.onMove += Move;
            unit.onAttack += Attack;
            unit.onTakeDamage += TakeDamage;
            unit.onDeath += Death;
        }
        else
        {
            Debug.Log("No animator found");
        }
    }

    public void Move()
    {
        anim.SetBool("Moving", true);
    }
    
    public void StopMoving()
    {
        anim.SetBool("Moving", false);
    }

    public void Attack()
    {
        anim.SetTrigger("Attacking");
    }

    public void TakeDamage()
    {
        anim.SetTrigger("Damaged");
    }

    public void Death()
    {
        anim.SetBool("Dead", true);
    }

    private void OnDisable()
    {
        if (GetComponentInChildren<Animator>())
        {
            unit.onMove -= Move;
            unit.onAttack -= Attack;
            unit.onTakeDamage -= TakeDamage;
            unit.onDeath -= Death;
        }
    }
}
