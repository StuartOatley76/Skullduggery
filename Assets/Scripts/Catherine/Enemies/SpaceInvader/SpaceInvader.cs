using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Space Invader behaviour
/// </summary>

public class SpaceInvader : BaseEnemy
{
    public static event EventHandler SpaceInvaderShoot;

    [Header("Bullet Info")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootingRange;

    private float lastShot;

    private void Update()
    {
        if (currentHealth <= 0) 
        { 
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if (player != null && isAwake)
        {
            // compares distance to player with shooting range
            if(Vector3.Distance(transform.position, player.position) < shootingRange)
            {
                // within range, check fire rate
                if(Time.time > fireRate + lastShot)
                {
                    // shoot
                    Attack();
                }
            }
            else
            {
                // move towards enemy
                if (enemy)
                    enemy.SetDestination(player.position);
            }
        }

        // play shielded/unshielded anims
        if (shielded)
        {
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedAttackAnim.name))
            {
                animator.Play(shieldedAnim.name);
            }
        }
        else if(!shielded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim.name))
            {
                animator.Play(baseAnim.name);
            }
        }
    }

    private void Attack()
    {
        // space invade shoot event
        SpaceInvaderShoot?.Invoke(this, EventArgs.Empty);

        // set shielded/unshielded attack anims
        if(shielded)
        {
            animator.Play(shieldedAttackAnim.name);
        }
        else if(!shielded)
        {
            animator.Play(attackAnim.name);
        }

        // spawn a bullet and set velocity in player direction
        var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation, gameObject.transform);
        bullet.GetComponent<Rigidbody>().velocity = (player.transform.position - gameObject.transform.position) * bulletSpeed;

        // set destination to current position, maintains positon whilst shooting
        if (enemy)
            enemy.SetDestination(transform.position);

        // set last shot time
        lastShot = Time.time;
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
