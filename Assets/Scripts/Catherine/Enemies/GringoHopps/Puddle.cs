using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Puddle behaviour spawned by frog
/// </summary>

public class Puddle : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private float damage;

    private void Start()
    {
        // adjust y pos
        transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);

        // destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // destroy puddle when room is cleared
        if(BaseEnemy.enemyCounter <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // damage player on collision
        var player = other.gameObject.GetComponent<CharacterController>();
        if (player && player.CanTakeDamage)
        {
            BaseEnemy.OnDamagePlayer(this, EventArgs.Empty);
            player.DamageEnemy(damage);
        }
        // ignore any other collision
        else
        {
            Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), other);
        }
        
    }
}
