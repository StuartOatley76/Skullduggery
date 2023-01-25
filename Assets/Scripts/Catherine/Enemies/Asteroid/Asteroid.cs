using UnityEngine;
using System;

/// <summary>
/// Asteroid behaviour
/// </summary>

public class Asteroid : BaseEnemy
{
    // asteroid events
    public static EventHandler AsteroidCrack;

    [SerializeField] private AnimationClip damageAnim2;
    [SerializeField] private AnimationClip damageAnim3;
    [SerializeField] private GameObject asteroidChunks;

    private int cracked = 0;

    private float damageRate;
    [SerializeField] private float startDamageRate;

    private bool isDamaging;

    private void Start()
    {
        // resets damage rate
        damageRate = startDamageRate;
    }

    private void Update()
    {
        if (player != null && currentHealth > 0 && isAwake) 
        { 
            // set player position as navmesh destination
            enemy.SetDestination(player.position);
        }

        if(currentHealth <= 0)
        {
            // spawn asteroid chunks
            Instantiate(asteroidChunks, gameObject.transform.position, gameObject.transform.rotation);

            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if(currentHealth <= (health * 0.25f))
        {
            // sets cracked state three
            if (cracked != 3)
            {
                cracked = 3;

                // asteroid crack event
                AsteroidCrack?.Invoke(this, EventArgs.Empty);
            }

            // play cracked state three anim
            animator.Play(damageAnim3.name);
        }
        else if(currentHealth <= (health * 0.5f))
        {
            // sets cracked state two
            if (cracked != 2)
            {
                cracked = 2;

                // asteroid crack event
                AsteroidCrack?.Invoke(this, EventArgs.Empty);
            }

            // play cracked state two anim
            animator.Play(damageAnim2.name);
        }
        else if(currentHealth <= (health * 0.75f))
        {
            // sets cracked state one
            if (cracked != 1)
            {
                cracked = 1;

                // asteroid crack event
                AsteroidCrack.Invoke(this, EventArgs.Empty);
            }

            // play cracked state one anim
            animator.Play(damageAnim.name);
        }
        else
        {
            // play shielded/unshielded base anims
            if (shielded)
            {
                animator.Play(shieldedAnim.name);
            }
            else if (!shielded)
            {
                animator.Play(baseAnim.name);
            }
        }

        // damage cooldown
        if(isDamaging)
        {
            damageRate -= Time.deltaTime;
        }

        // reset damage cooldown
        if(damageRate <= 0)
        {
            isDamaging = false;
            damageRate = startDamageRate;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // damage player on collision and after damage cooldown
        if (!isDamaging)
        {
            isDamaging = true;

            var player = collision.gameObject.GetComponent<CharacterController>();
            if (player && player.CanTakeDamage)
            {
                OnDamagePlayer(this, EventArgs.Empty);
                player.DamageEnemy(damage);
            }
        }

    }
}
