using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Player bullet behaviour
/// </summary>

public class Bullet : MonoBehaviour
{
    // bullet events
    public static event EventHandler DamageEnemy;
    public static event EventHandler ShieldHit;

    [SerializeField] private float baseBulletDamage;
    [SerializeField] private Animator bubbleAnimator;
    public Vector3 velocity;

    private float totalBulletDamage;

    private void Start()
    {
        // initialise velocity
        velocity = GetComponent<Rigidbody>().velocity;
    }

    private void Update()
    {
        // calculates bullet damage with damage stat multiplier
        totalBulletDamage = (baseBulletDamage * StatMultiplier.Instance.BulletDamageIncreasePerSecond * Time.deltaTime) + baseBulletDamage;
    }

    private void FixedUpdate()
    {
        // maintains velocity
        GetComponent<Rigidbody>().velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ignore collision with the player.
        if(collision.gameObject.GetComponent<CharacterController>())
        {
            Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
        }

        switch (collision.gameObject.tag)
        {
            // hit enemy
            case "Enemy":
                var enemy = collision.gameObject.GetComponent<BaseEnemy>();
                var shieldEnemy = collision.gameObject.GetComponent<SpaceInvaderShield>();

                // shielded hit
                if (enemy.shielded && !shieldEnemy)
                {
                    OnShieldHit(this, EventArgs.Empty);
                    enemy.OnShieldHit();
                }
                // boss hit
                if(!enemy.shielded && enemy.GetComponent<CowboyBoss>() != null)
                {
                    // check if boss can take damage
                    if (enemy.GetComponent<CowboyBoss>().canTakeDamage)
                    {
                        // damage boss with damage to boss multiplier
                        OnDamageEnemy(this, EventArgs.Empty);
                        enemy.OnDamaged(totalBulletDamage * StatMultiplier.Instance.DamageToBossMultiplier);
                    }
                }
                // unshielded hit
                else if (!enemy.shielded || shieldEnemy)
                {
                    // damage enemy
                    OnDamageEnemy(this, EventArgs.Empty);
                    enemy.OnDamaged(totalBulletDamage * StatMultiplier.Instance.StandardEnemyDamageMultiplier);
                    
                }
                DestroyBubble();
                break;
            // destroy bubble on envrionment collision
            case "Environment":
                DestroyBubble();
                break;
            // ignore any other collision
            default:
                Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), collision.gameObject.GetComponent<BoxCollider>());
                break;
        }
    }

    // enemy hit event
    public void OnDamageEnemy(object sender, EventArgs e)
    {
        DamageEnemy?.Invoke(sender, e);
    }

    // shield hit event
    public void OnShieldHit(object sender, EventArgs e)
    {
        ShieldHit?.Invoke(sender, e);
    }

    private void DestroyBubble()
    {
        // turns off damage when death anim is playing
        totalBulletDamage = 0;
        bubbleAnimator.Play("BubblePop");
    }
}
