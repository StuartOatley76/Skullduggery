using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls enemy bullet behaviour
/// </summary>

public class EnemyBullet : MonoBehaviour
{
    // stores the type of enemy bullet
    public enum EnemyType
    {
        SPACEINVADER,
        PONGBAT
    }
    public EnemyType enemyType;

    [SerializeField] private float bulletDamage;
    public float bulletSpeed;
    [SerializeField] private Animator bulletAnimator;

    private Vector3 velocity;
    private GameObject parent;
    private bool isTargeting = false;

    private void Start()
    {
        // initialise velocity
        velocity = GetComponent<Rigidbody>().velocity;
        
        // save parent game object
        parent = transform.parent.gameObject;
        // remove object from parent
        transform.parent = null;
    }

    private void FixedUpdate()
    {
        // sets velocity dependent on enemy type and state
        switch (enemyType)
        {
            case EnemyType.SPACEINVADER:
                // maintains bullet velocity
                GetComponent<Rigidbody>().velocity = velocity;
                break;
            case EnemyType.PONGBAT:
                if(!isTargeting)
                {
                    // maintains bullet velocity when not targeting parent
                    GetComponent<Rigidbody>().velocity = parent.GetComponent<PongBat>().velocity;
                }
                else
                {
                    // sets velocity to target parent
                    TargetParent();
                }
                break;
        }
    }

    private void Update()
    {
        // destroy if all enemies dead
        if (BaseEnemy.enemyCounter <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // checks collision behaviour
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                if (collision.gameObject == parent)
                {
                    switch (enemyType)
                    {
                        case EnemyType.SPACEINVADER:
                            // ignore collision with parent object
                            if (enemyType == EnemyType.SPACEINVADER)
                            {
                                Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                            }
                            break;
                        case EnemyType.PONGBAT:
                            // disable targeting on collision with parent object
                            if (isTargeting == true)
                            {
                                isTargeting = false;
                            }
                            break;
                    }
                }
                else
                {
                    switch (enemyType)
                    {
                        case EnemyType.SPACEINVADER:
                            // destoy bullet on enemy collison
                            Destroy(gameObject);
                            break;
                        case EnemyType.PONGBAT:
                            // ignore enemy collision
                            Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                            break;
                    }
                }
                break;
            case "Player":
                // deal damage on player collision
                var player = collision.gameObject.GetComponent<CharacterController>();
                if (player.CanTakeDamage)
                {
                    BaseEnemy.OnDamagePlayer(this, EventArgs.Empty);
                    // adjust damage value with damage multiplier
                    player.DamageEnemy(bulletDamage * StatMultiplier.Instance.EnemyBulletDamageMultiplier);
                }
                switch (enemyType)
                {
                    // destroy space invader bullet
                    case EnemyType.SPACEINVADER:
                        Destroy(gameObject);
                        break;
                    // set pong bullet to target parent
                    case EnemyType.PONGBAT:
                        TargetParent();
                        break;
                }
                break;
            default:
                switch(enemyType)
                {
                    case EnemyType.SPACEINVADER:
                        // destroy on any collision
                        Destroy(gameObject);
                        break;
                    case EnemyType.PONGBAT:
                        // target parent on any collision
                        TargetParent();
                        break;
                }
                break;
        }
    }

    private void TargetParent()
    {
        // starts targeting and moving towards parent
        if(!isTargeting) { isTargeting = true; }
        transform.position = Vector3.MoveTowards(transform.position, parent.transform.position, bulletSpeed * Time.deltaTime);
    }
}
