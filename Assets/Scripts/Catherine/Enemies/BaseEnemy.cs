using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

/// <summary>
/// Base class for enemies, stores base variables and behaviour
/// </summary>

public abstract class BaseEnemy : MonoBehaviour
{
    public GameObject[] pickups;
    public GameObject deathParticle;
    public GameObject shieldParticle;
    public float particleYPos;

    [Header("Base Stats")]
    public float health;
    public float damage;
    public float deathScore;
    public GameObject scoreFeedback;
    public bool deathEffects = true;
    public bool changeMass = true;

    [Header("Health UI")]
    public Slider healthBarSlider;
    public Image healthBarFill;
    public Material baseHealthMaterial;
    public Material shieldedHealthMaterial;

    [Header("Enemy Animations")]
    public AnimationClip baseAnim;
    public AnimationClip shieldedAnim;
    public AnimationClip attackAnim;
    public AnimationClip shieldedAttackAnim;
    public AnimationClip damageAnim;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public NavMeshAgent enemy;
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public bool shielded;
    [HideInInspector]
    public bool canPlayDamageAnim = true;
    [HideInInspector]
    public Rigidbody rb;


    // enemy events
    public static event EventHandler EnemyKilled;
    public static event EventHandler BossDeath;
    public static event EventHandler DamagePlayer;

    public static int enemyCounter = 0;

    private float healthDecimal;
    public bool isBoss = false;

    private InGameUI gameUI;

    public bool isAwake;

    public static GameObject[] enemies;
    public static bool bossDead;

    public void Awake()
    {
        // updates enemy counter
        enemyCounter += 1;

        // initialises variables
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameUI = player.GetComponentInChildren<InGameUI>();
        if(!isBoss)
        {
            currentHealth = health;
        }
        isAwake = false;

        // waits on awake so enemies can't hit player on spawn
        StartCoroutine(WaitOnAwake());
    }

    public void FixedUpdate()
    {
        if(bossDead)
        {
            currentHealth = 0;
        }

        // find all enemy game objects
         enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyShield in enemies)
        {
            // activate shields when shield enemy is alive
            if (enemyShield.GetComponent<SpaceInvaderShield>())
            {
                if (changeMass && !isBoss)
                {
                    // change mass so shielded enemies can be pushed by bullets
                    rb.mass = 1;
                }
                shielded = true;

                // prevent shield enemies from being pushed
                enemyShield.GetComponent<Rigidbody>().mass = 100;
                break;
            }
            // disable shields
            else
            {
                if (changeMass && !isBoss)
                {
                    // reset mass so enemies can't be pushed
                    rb.mass = 100;
                }
                shielded = false;
            }
        }

        // updates health bar
        if (healthBarSlider != null)
        {
            healthDecimal = currentHealth / health;
            healthBarSlider.value = healthDecimal;
        }

        // changes health bar material when shielded/unshielded
        if (shielded)
        {
            if (shieldedHealthMaterial != null)
            {
                healthBarFill.material = shieldedHealthMaterial;
            }
        }
        else if (!shielded)
        {
            if (baseHealthMaterial != null)
            {
                healthBarFill.material = baseHealthMaterial;
            }
        }
    }

    public void OnDamaged(float _damageAmount)
    {
        // play damaged animation
        if (canPlayDamageAnim && damageAnim != null && animator != null)
        {
            animator.Play(damageAnim.name);
        }

        // take away health
        currentHealth -= _damageAmount;
    }

    public void OnEnemyKilled(object sender, EventArgs e)
    {
        // updates enemy counter for rooms
        enemyCounter -= 1;
        RoomInformation.activeRoom.EnemyKilled(gameObject);

        if (deathEffects)
        {
            // random chance to spawn random pickup
            int chance = UnityEngine.Random.Range(0, 10);
            if (chance < 2)
            {
                int randPickup = UnityEngine.Random.Range(0, pickups.Length);
                GameObject pickup = Instantiate(pickups[randPickup], transform.position, transform.rotation);
                // adjust y position
                pickup.transform.position = new Vector3(pickup.transform.position.x, particleYPos, pickup.transform.position.z);
            }

            // play death particle effects
            if (deathParticle != null)
            {
                GameObject death = Instantiate(deathParticle, transform.position, transform.rotation);
                death.transform.position = new Vector3(death.transform.position.x, particleYPos, death.transform.position.z);
            }
        }

        // get the current enemy score
        var score = deathScore * ComboSystem.comboMultiplier * StatMultiplier.Instance.ScoreMultiplier;

        // create score feedback
        var feedback = Instantiate(scoreFeedback, transform.position, Quaternion.identity);
        feedback.GetComponentInChildren<Text>().text = score.ToString();

        // update total score
        gameUI.UpdateScore(score);

        // send enemy killed event
        if(!isBoss)
        {
            EnemyKilled?.Invoke(sender, e);
        }
        // send boss killed event
        else
        {
            bossDead = true;
            OnBossDied(this, EventArgs.Empty);
        }

        // destroy enemy
        Destroy(gameObject);
    }

    public Vector3 RandomTarget(Vector3 origin, float distance)
    {
        // get random direction
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * distance;
        randomDir += origin;

        // find position on navmesh using the random direction
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomDir, out navMeshHit, distance, -1);
        return navMeshHit.position;
    }

    public IEnumerator WaitOnAwake()
    {
        // prevents insta hits on player spawn
        yield return new WaitForSeconds(1);
        isAwake = true;
    }

    public static void OnBossDied(object sender, EventArgs e)
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<BaseEnemy>().currentHealth = 0;
        }
        BossDeath?.Invoke(sender, e);
    }

    // damage player event
    public static void OnDamagePlayer(object sender, EventArgs e)
    {
        DamagePlayer?.Invoke(sender, e);
    }

    public void OnShieldHit()
    {
        // play shielded particle effect
        if (shieldParticle != null)
        {
            GameObject shield = Instantiate(shieldParticle, transform.position, transform.rotation);
            shield.transform.position = new Vector3(shield.transform.position.x, particleYPos, shield.transform.position.z);
        }
    }
}
