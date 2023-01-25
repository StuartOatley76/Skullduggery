using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
/// Frog enemy behaviour
/// </summary>

public class GringoHopps : BaseEnemy
{
    [Header("Jump Info")]
    [SerializeField] private float jumpRate;
    [SerializeField] private float jumpRadius;
    [SerializeField] private AnimationClip jumpAnim;
    [SerializeField] private AnimationClip shieldedJumpAnim;
    [SerializeField] private AnimationClip endJumpAnim;
    [SerializeField] private AnimationClip shieldedEndJumpAnim;

    private Vector3 target;
    private float lastJump;

    private void Update()
    {
        if (currentHealth <= 0)
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if (isAwake)
        {
            // start jump
            if (Time.time > jumpRate + lastJump)
            {
                Jump();
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName(jumpAnim.name) || animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedJumpAnim.name))
            {
                // start attack
                canPlayDamageAnim = false;
                Attack();
            }
        }

        if (shielded)
        {
            // play shielded base anim and set velocity to zero when not jumping/attacking
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedAttackAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedJumpAnim.name)
                && !animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedEndJumpAnim.name))
            {
                animator.Play(shieldedAnim.name);
                rb.velocity = Vector3.zero;
            }
        }
        else if (!shielded)
        {
            // play unshielded base anim and set velocity to zero when not jumping/attacking
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedAttackAnim.name)
                && !animator.GetCurrentAnimatorStateInfo(0).IsName(jumpAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(endJumpAnim.name)
                && !animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedJumpAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedEndJumpAnim.name))
            {
                animator.Play(baseAnim.name);
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Jump()
    {
        // play shielded/unshielded jump attack anims
        if (shielded)
        {
            animator.Play(shieldedAttackAnim.name);
        }
        else if (!shielded)
        {
            animator.Play(attackAnim.name);
        }

        // set a random target within the jump radius
        target = RandomTarget(transform.position, jumpRadius);

        // set last jump time
        lastJump = Time.time;
    }

    private void Attack()
    {
        // move towards target destination
        enemy.SetDestination(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // damage player on collision
        var player = collision.gameObject.GetComponent<CharacterController>();
        if (player && player.CanTakeDamage)
        {
            OnDamagePlayer(this, EventArgs.Empty);
            player.DamageEnemy(damage);
        }
    }
}
