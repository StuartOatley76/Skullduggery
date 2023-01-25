using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base class for traps
/// </summary>

public class BaseTrap : MonoBehaviour
{
    // trap events
    public static event EventHandler ActiveTrap;

    public float damage;

    [Header("Trap Animations")]
    public AnimationClip baseAnim;
    public AnimationClip trapAnim;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public CharacterController player;

    private bool isEnabled;

    private void Awake()
    {
        // intialise animator
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        // enabled when enemies in room
        if(BaseEnemy.enemyCounter > 0)
        {
            isEnabled = true;
        }
        // disabled when room is clear
        else
        {
            isEnabled = false;
        }

        // play base animation when not activated
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName(trapAnim.name))
        {
            animator.Play(baseAnim.name);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // activate trap on player collision
        player = collider.gameObject.GetComponent<CharacterController>();
        if (player && isEnabled)
        {          
            ActivateTrap();
        }
    }

    public virtual void ActivateTrap()
    {
        // activated trap event
        ActiveTrap?.Invoke(this, EventArgs.Empty);

        // deal player damage
        if(player.CanTakeDamage)
        {
            player.DamageEnemy(damage);
            BaseEnemy.OnDamagePlayer(this, EventArgs.Empty);
        }
    }
}