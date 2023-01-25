using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Asteroid chunks behaviour, spawned upon Asteroid death
/// </summary>

public class AsteroidChunks : BaseEnemy
{
    private GameObject parentAsteroid;

    private void Start()
    {
        if (transform.parent != null)
        {
            // save parent object for reference
            parentAsteroid = transform.parent.gameObject;

            // remove parent
            transform.parent = null;
        }
    }

    private void Update()
    {
        // ensure parent asteroid is destroyed
        if( transform.parent == null && parentAsteroid != null)
        {
            Destroy(parentAsteroid);
        }

        if (currentHealth <= 0)
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if (player != null)
        {
            // set player as navmesh destination
            enemy.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // damage player on collison
        var player = collision.gameObject.GetComponent<CharacterController>();
        if (player && player.CanTakeDamage)
        {
            OnDamagePlayer(this, EventArgs.Empty);
            player.DamageEnemy(damage);

            // destroy object when collided with player
            OnEnemyKilled(this, EventArgs.Empty);
        }
    }
}
