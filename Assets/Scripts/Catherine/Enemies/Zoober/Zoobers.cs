using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Zoobers behaviour
/// </summary>

public class Zoobers : BaseEnemy
{
    // zoober events
    public static event EventHandler CarZoom;

    [SerializeField] private float moveSpeed;
    private Direction direction;

    // stores direction
    private enum Direction
    {
        DOWN,
        UP
    }

    private void Start()
    {
        // sets initial direction to down
        direction = Direction.DOWN;
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else
        {
            // car constantly moving
            Move();
        }

        if (shielded)
        {
            // play shielded directional anims
            switch(direction)
            {
                case Direction.DOWN:
                    animator.Play(shieldedAnim.name);
                    break;
                case Direction.UP:
                    animator.Play(shieldedAttackAnim.name);
                    break;
            }
        }
        else if (!shielded && !animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name))
        {
            // play unshielded directional anims
            switch (direction)
            {
                case Direction.DOWN:
                    animator.Play(baseAnim.name);
                    break;
                case Direction.UP:
                    animator.Play(attackAnim.name);
                    break;
            }
        }
    }

    private void Move()
    {
        switch (direction)
        {
            // moves enemy on navmesh in isometric down
            case Direction.DOWN:
                enemy.Move((Vector3.back + Vector3.left) * moveSpeed * Time.deltaTime);
                break;
            // moves enemy on navmesh in isometric up
            case Direction.UP:
                enemy.Move((Vector3.forward + Vector3.right) * moveSpeed * Time.deltaTime);
                break;
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

        // swap direction on collision with anything but bullets/enemies
        else if(collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy")
        {
           SwapDirection();
        }
    }

    private void SwapDirection()
    {
        // car zoom event
        CarZoom?.Invoke(this, EventArgs.Empty);

        // set to opposite direction
        switch(direction)
        {
            case Direction.DOWN:
                direction = Direction.UP;
                break;
            case Direction.UP:
                direction = Direction.DOWN;
                break;
        }
    }
}
