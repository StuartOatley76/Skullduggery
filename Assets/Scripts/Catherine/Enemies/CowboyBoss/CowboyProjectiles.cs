using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls Cowboy projectile behaviour
/// </summary>

public class CowboyProjectiles : MonoBehaviour
{
    // stores attack projectile type
    public enum ProjectileType
    {
        YEEHAW,
        ROOTINTOOTIN
    }
    public ProjectileType projectileType;

    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileMultiplier;

    private Vector3 velocity;
    private GameObject player;

    private void Start()
    {
        // initialises velocity and player
        velocity = GetComponent<Rigidbody>().velocity;
        player = GameObject.FindGameObjectWithTag("Player");

        // remove parent
        transform.parent = null;
    }

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.YEEHAW:
                // maintains velocity
                GetComponent<Rigidbody>().velocity = velocity;
                break;
            case ProjectileType.ROOTINTOOTIN:
                // increases velocity with projectile multiplier
                GetComponent<Rigidbody>().velocity = velocity * projectileMultiplier;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // checks collision
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                // ignore enemy collison
                Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;
            case "Player":          
                // damage player on collision
                if (player.GetComponent<CharacterController>().CanTakeDamage)
                {
                    BaseEnemy.OnDamagePlayer(this, EventArgs.Empty);
                    player.GetComponent<CharacterController>().DamageEnemy(projectileDamage);
                }
                // destroy projectile
                Destroy(gameObject);
                break;
            default:
                // destroy projectile
                Destroy(gameObject);
                break;
        }
    }
}
