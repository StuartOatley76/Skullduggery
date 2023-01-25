using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Cowboy boss behaviour
/// </summary>

public class CowboyBoss : BaseEnemy
{
    // cowboy boss events
    public static event EventHandler BossShoot;
    public static event EventHandler BossSlam;

    [HideInInspector]
    public bool isAttacking;

    [SerializeField] private GameObject yeehawProjectile;
    [SerializeField] private GameObject rootinTootinProjectile;

    public int numberOfProjectiles, projectileSpeed;

    [SerializeField] private AnimationClip slamAttackAnim;
    [SerializeField] private AnimationClip slamAttackShieldAnim;

    [SerializeField] private float attackRate, attackRadius;
    [SerializeField] private float wanderRadius;

    [SerializeField] private float numberOfWaves;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int firstPhaseEnemyQuantity, secondPhaseEnemyQuantity;

    [HideInInspector]
    public bool canTakeDamage = true;

    private AudioSource audioSource;

    private float lastAttack;
    private bool secondPhase;

    private void Start()
    {
        // set boss
        isBoss = true;

        // reset phase
        animator.SetBool("isSecondPhase", secondPhase);
        secondPhase = false;

        // initialise audio source
        audioSource = GetComponent<AudioSource>();

        // set health considering powerups
        health = health * StatMultiplier.Instance.BossHealthMultiplier;
        currentHealth = health;
    }

    private void Update()
    {
        // disable damage whilst attacking
        if(isAttacking && canTakeDamage)
        {
            canTakeDamage = false;
        }
        // enable damage when not attacking
        else if (!isAttacking && !canTakeDamage)
        {
            canTakeDamage = true;
        }

        if (currentHealth <= 0)
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        else if (Time.time > attackRate + lastAttack && !isAttacking)
        {
            // start attack after cooldown
            Attack();
        }
        else if(player != null && !isAttacking)
        {
            if (!audioSource.isPlaying)
            {
                // play footsteps audio
                audioSource.Play();
            }

            // set player as navmesh destination
            enemy.SetDestination(player.position);
        }

        // set second phase at half health
        if (currentHealth <= (health / 2))
        {
            secondPhase = true;
            animator.SetBool("isSecondPhase", secondPhase);
        }

        // play shielded/unshielded anims
        if (shielded && !isAttacking)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(shieldedAttackAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(slamAttackShieldAnim.name))
            {               
                animator.Play(shieldedAnim.name);
            }
        }
        else if (!shielded && !isAttacking)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(damageAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim.name) && !animator.GetCurrentAnimatorStateInfo(0).IsName(slamAttackAnim.name))
            {
                animator.Play(baseAnim.name);
            }
        }
    }

    private void Attack()
    {
        // set animator bool
        animator.SetBool("isAttackActive", true);

        // stop footsteps audio
        audioSource.Stop();

        // stop navmesh path
        enemy.ResetPath();

        // start random attack
        isAttacking = true;
        var randAttack = UnityEngine.Random.Range(0, 2);

        // check enemy counter, don't spawn more enemies if previous enemies not killed
        if ( randAttack == 0 && enemyCounter < 2)
        {
            SlamAttack();
        }
        else
        {
            // yeehaw first phase attack
            if (!secondPhase)
            {
                StartCoroutine(YeehawAttack());
            }
            // root toot second phase attack
            else
            {
                StartCoroutine(RootTootAttack());
            }
        }
    }

    private IEnumerator YeehawAttack()
    {
        // play shielded/unshielded yeehaw attack anims
        if (shielded)
        {
            animator.Play(shieldedAttackAnim.name);
        }
        else
        {
            animator.Play(attackAnim.name);
        }

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            rb.velocity = Vector3.zero;

            // wait between each shot
            yield return new WaitForSeconds(0.8f);

            // boss shoot event
            BossShoot?.Invoke(this, EventArgs.Empty);

            // create projectile that targets the player
            var proj = Instantiate(yeehawProjectile, transform.position, transform.rotation, transform);
            proj.GetComponent<Rigidbody>().velocity = (player.transform.position - gameObject.transform.position) * projectileSpeed;

            if(i == numberOfProjectiles -1)
            {
                // set last attack time
                lastAttack = Time.time;
                isAttacking = false;
            }
        }

        // set animator bool
        animator.SetBool("isAttackActive", false);
    }

    private IEnumerator RootTootAttack()
    {
        // play shielded/unshielded root toot attack anims
        if (shielded)
        {
            animator.Play(shieldedAttackAnim.name);
        }
        else
        {
            animator.Play(attackAnim.name);
        }

        
        for (int i = 0; i < numberOfWaves; i++)
        {
            rb.velocity = Vector3.zero;

            // wait between each wave of projectiles
            yield return new WaitForSeconds(0.8f);

            // boss shoot event
            BossShoot?.Invoke(this, EventArgs.Empty);

            // create range of projectiles in a radius
            for (int j = 0; j < numberOfProjectiles; j++)
            {
                var proj = Instantiate(rootinTootinProjectile, transform.position, transform.rotation, transform);
                proj.GetComponent<Rigidbody>().velocity = EightAxisUtility.direction[j] * projectileSpeed;
            }

            if (i == numberOfWaves - 1)
            {
                // set last attack time
                lastAttack = Time.time;
                isAttacking = false;
            }
        }

        // set animator bool
        animator.SetBool("isAttackActive", false);
    }

    private void SlamAttack()
    {
        // boss slam event
        BossSlam?.Invoke(this, EventArgs.Empty);

        // play shielded/unshielded slam attack anims
        if (shielded)
        {
            animator.Play(slamAttackShieldAnim.name);
        }
        else
        {
            animator.Play(slamAttackAnim.name);
        }

        // spawn first/second pahse enemies
        if (!secondPhase)
        {
            SummonEnemies(firstPhaseEnemyQuantity);
        }
        else
        {
            SummonEnemies(secondPhaseEnemyQuantity);
        }

        // set last attack time
        lastAttack = Time.time;
        isAttacking = false;
    }


    // spawns enemies at random locations within the attack radius on the navmesh
    private void SummonEnemies(int enemyAmount)
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            // gets random postion
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere * attackRadius;
            randomDir += transform.position;

            bool foundNavMesh = false;
            float range = 0.25f;

            // finds position on navmesh
            NavMeshHit hit;
            do
            {
                if (NavMesh.SamplePosition(randomDir, out hit, range, 1))
                {
                    randomDir = hit.position;
                    foundNavMesh = true;
                }
                else
                {
                    range += range;
                }
            } while (!foundNavMesh);

            // instantiate random enemy on navmesh position
            int j = UnityEngine.Random.Range(0, enemies.Length);
            Instantiate(enemies[j], randomDir, transform.rotation);
        }
    }
}
