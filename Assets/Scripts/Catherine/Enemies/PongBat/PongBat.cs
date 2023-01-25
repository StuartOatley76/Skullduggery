using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Pong bat behaviour
/// </summary>

public class PongBat : BaseEnemy
{
    // pong bat events
    public static event EventHandler pongShoot;

    [HideInInspector]
    public Vector3 velocity;

    [SerializeField] private GameObject pongBatBullet;
    [SerializeField] private float moveSpeed;

    // stores direction
    private Direction direction;
    private enum Direction
    {
        LEFT,
        RIGHT
    }
    private float bulletSpeed;

    private void Start()
    {
        // sets initial direction to left
        direction = Direction.LEFT;
        // initialises bullet speed
        bulletSpeed = pongBatBullet.GetComponent<EnemyBullet>().bulletSpeed;

        // start attack
        Attack();       
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            // destroy bullet upon death
            Destroy(pongBatBullet);

            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }

        // constantly move
        Move();

        // play shielded/unshielded anims
        if (shielded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedAttackAnim.name))
            {
                animator.Play(shieldedAnim.name);
            }
        }
        else if (!shielded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim.name))
            {
                animator.Play(baseAnim.name);
            }
        }
    }

    private void Move()
    {
        switch (direction)
        {
            // moves enemy on navmesh in isometric left
            case Direction.LEFT:
                transform.Translate((Vector3.left) * moveSpeed * Time.deltaTime);
                break;
            // moves enemy on navmesh in isometric right
            case Direction.RIGHT:
                transform.Translate((Vector3.right) * moveSpeed * Time.deltaTime);
                break;
        }
    }

    private void Attack()
    {
        // pong shoot event
        pongShoot?.Invoke(this, EventArgs.Empty);

        switch (direction)
        {
            // sets velocity in isometric up direction when moving left
            case Direction.LEFT:
                velocity = (Vector3.forward + Vector3.right) * bulletSpeed;
                break;
            // sets velocity in isometric down direction when moving right
            case Direction.RIGHT:
                velocity = (Vector3.back + Vector3.left) * bulletSpeed;
                break;
        }

        // play shielded/unshielded attack anims
        if (shielded)
        {
            animator.Play(shieldedAttackAnim.name);
        }
        else
        {
            animator.Play(attackAnim.name);
        }
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
        
        // attack on collison with pong bullet
        if(collision.gameObject == pongBatBullet)
        {
            Attack();
        }
        // swap direction on any other collision
        else
        {
            SwapDirection();
        }
    }

    private void SwapDirection()
    {
        // set to opposite direction
        switch (direction)
        {
            case Direction.LEFT:
                direction = Direction.RIGHT;
                break;
            case Direction.RIGHT:
                direction = Direction.LEFT;
                break;
        }
    }
}
