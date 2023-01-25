using UnityEngine;
using System;

/// <summary>
/// Pacman enemy behaviour
/// </summary>

public class PacMan : BaseEnemy
{
    [SerializeField] private AnimationClip edibleAnim;
    [SerializeField] private float healthIncrease;

    private void Update()
    {
        if (currentHealth <= 0)
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if (player != null && !LastOneStanding() && isAwake)
        {
            // set player as navmesh destination
            enemy.SetDestination(player.position);
        }

        // play shielded/unshielded anims
        if (shielded)
        {
            animator.Play(shieldedAnim.name);
        }
        else if (!shielded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(edibleAnim.name))
            {
                animator.Play(baseAnim.name);
            }
        }

        // when pacman is the last remaining enemy, become edible
        if (LastOneStanding())
        {
            Edible();
        }
    }

    private bool LastOneStanding()
    {
        // find all enemies
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject pacman in enemies)
        {
            // check if there are other enemy types
            if(!pacman.GetComponent<PacMan>())
            {
                return false;
            }
        }

        // pacman is last remaining enemy type
        return true;
    }

    private void Edible()
    {
        if (healthBarSlider != null)
        {
            // remove health bar
            Destroy(healthBarSlider.gameObject);

            // remove enemy tag
            gameObject.tag = "Untagged";
        }

        // move away from the player
        if(player != null)
        {
            enemy.SetDestination((gameObject.transform.position - player.transform.position).normalized + gameObject.transform.position);
        }

        // disable death effects
        deathEffects = false;

        // play edible anim
        animator.Play(edibleAnim.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // on player collision
        var player = collision.gameObject.GetComponent<CharacterController>();
        if (player)
        {
            // increase player health if edible and kill pacman
            if(LastOneStanding())
            {
                player.CurrentHealth += healthIncrease;
                if (player.CurrentHealth > player.Health)
                {
                    player.CurrentHealth = player.Health;
                }
                // enemy killed event
                OnEnemyKilled(this, EventArgs.Empty);
            }
            // damage player
            else if(player.CanTakeDamage)
            {
                OnDamagePlayer(this, EventArgs.Empty);
                player.DamageEnemy(damage);
            }
        }
    }
}
