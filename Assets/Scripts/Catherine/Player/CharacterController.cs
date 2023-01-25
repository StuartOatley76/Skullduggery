using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Character Contoller behaviour - stores all player data and receives input from the controller
/// </summary>

public class CharacterController : MonoBehaviour
{
    // player events
    public static event EventHandler PlayerKilled;
    public static event EventHandler PlayerDashing;

    public float inputDeadzone;

    [SerializeField] private bool invincibleToggle = false;

    [SerializeField] private float health;
    public float Health { get { return health; } set { health = value; } } // public total health accessor

    [SerializeField] private float currentHealth;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } } // public current health accessor

    [SerializeField] private float moveSpeed, dashSpeed, dashCooldown;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } } // public move speed accessor

    private bool canTakeDamage;
    public bool CanTakeDamage { get { return canTakeDamage; } set { canTakeDamage = value; } } // public take damage bool accessor

    [SerializeField] private float startDashTime;

    [SerializeField] private Transform weaponPoint;
    [SerializeField] private float yStartPos;

    [SerializeField] private Color damageTint;

    [SerializeField] private GameObject buttonPrompt;

    [Header("Player Animations")]
    [SerializeField] private AnimationClip playerIdleSE;
    [SerializeField] private AnimationClip playerRunNE;
    [SerializeField] private AnimationClip playerRunSE;
    [SerializeField] private AnimationClip playerShootNE;
    [SerializeField] private AnimationClip playerShootSE;
    [SerializeField] private AnimationClip playerRunShootNE;
    [SerializeField] private AnimationClip playerRunShootSE;
    [SerializeField] private AnimationClip playerDashNE;
    [SerializeField] private AnimationClip playerDashSE;

    [Header("Bubble Spawn Positions")]
    [SerializeField] private Vector3[] NE_SpawnPositions;
    [SerializeField] private Vector3[] NW_SpawnPositions;
    [SerializeField] private Vector3[] SE_SpawnPositions;
    [SerializeField] private Vector3[] SW_SpawnPositions;

   [HideInInspector]
    public Animator playerAnims;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool isShooting;
    [HideInInspector]
    public bool isDashing = false;
    [HideInInspector]
    public int lastDirection;

    private Rigidbody playerRigidbody;
    private Vector2 movement;
    private float dashTime;
    private float lastDash;

    private bool isInputEnabled = true;

    private BubbleGun bubbleGun;
    private AudioSource audioSource;

    private void Start()
    {
        // subscribe listeners
        BaseEnemy.DamagePlayer += DamagePlayerListener;

        // reset values
        currentHealth = health;
        dashTime = startDashTime;
        lastDash = dashCooldown;
        canTakeDamage = true;
        buttonPrompt.SetActive(false);

        // initialise variables
        playerRigidbody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnims = GetComponent<Animator>();
        bubbleGun = GetComponentInChildren<BubbleGun>();
        audioSource = GetComponent<AudioSource>();

        // Invincibility for testing
        if (invincibleToggle)
        {
            canTakeDamage = false;
        }
    }

    private void Update()
    {
        // adjust y pos
        if(transform.position.y != yStartPos)
        {
            transform.position = new Vector3(transform.position.x, yStartPos, transform.position.z);
        }

        // checks if paused
        if (Time.timeScale > 0)
        {
            // checks if dead
            if (currentHealth <= 0)
            {
                // player killed event
                OnPlayerKilled(this, EventArgs.Empty);

                Camera.main.gameObject.transform.parent.parent = null;
                BaseEnemy.enemyCounter = 0;
                Destroy(gameObject);
            }
            // input
            if (isInputEnabled)
            {
                PlayerInput();
            }
            else
            {
                // default movement to zero
                movement = Vector2.zero;
            }

            // set invincibility with pickup or debug toggle
            if(StatMultiplier.Instance.Invincibility && !invincibleToggle)
            {
                canTakeDamage = false;
            }
            else if(!isDashing && !invincibleToggle)
            { 
                canTakeDamage = true; 
            }

            // sets up dash
            if (isDashing)
            {
                // assigns correct anims and sprites for dash
                EightAxisUtility.SetSprite(playerAnims, playerDashNE.name, playerDashSE.name, movement);
                EightAxisUtility.SpriteFlip(spriteRenderer, movement);

                // apply velocity in last direction
                playerRigidbody.velocity = EightAxisUtility.direction[lastDirection] * dashSpeed;
                
                // set dash time
                dashTime -= Time.deltaTime;

                // player invincible whilst dashing
                if (!invincibleToggle && !StatMultiplier.Instance.Invincibility)
                {
                    canTakeDamage = false;
                }

                // disable movement whilst dashing
                DisablePlayerMovement(isDashing);
            }
            // dash ended
            if (dashTime <= 0)
            {
                // reset
                dashTime = startDashTime;
                isDashing = false;
                DisablePlayerMovement(isDashing);
            }

            if (isShooting && movement != Vector2.zero)
            {
                // assigns correct anims and sprites for running and shooting
                EightAxisUtility.SetSprite(playerAnims, playerRunShootNE.name, playerRunShootSE.name, bubbleGun.shootingDirection);
                EightAxisUtility.SpriteFlip(spriteRenderer, bubbleGun.shootingDirection);
            }
            else if (isShooting)
            {
                // assigns correct anims and sprites for shooting
                EightAxisUtility.SetSprite(playerAnims, playerShootNE.name, playerShootSE.name, bubbleGun.shootingDirection);
                EightAxisUtility.SpriteFlip(spriteRenderer, bubbleGun.shootingDirection);
            }

            // update bullet spawn positions
            AdjustBulletSpawnPosition();
        }
    }

    // receives the controller inputs
    private void PlayerInput()
    {
        movement = new Vector2(Input.GetAxisRaw("Gamepad_Horizontal"), Input.GetAxisRaw("Gamepad_Vertical"));
        lastDirection = EightAxisUtility.EightAxisDirection(movement);

        // movement 
        MovePlayer();

        if (!isShooting)
        {
            // assigns correct anims and sprites for running
            EightAxisUtility.SetSprite(playerAnims, playerRunNE.name, playerRunSE.name, movement);
            EightAxisUtility.SpriteFlip(spriteRenderer, movement);
        }

        // dash input
        if ((Input.GetAxisRaw("Gamepad_Dash") > 0) && Time.time > dashCooldown + lastDash && movement != Vector2.zero)
        {
            // player dash event
            PlayerDashing?.Invoke(this, EventArgs.Empty);

            isDashing = true;
            lastDash = Time.time;
        }
    }

    // moves the player
    private void MovePlayer()
    {
        // sets movement to zero if in controller deadzone
        if (movement.magnitude < inputDeadzone)
        {
            movement = Vector2.zero;

            // stop footstep audio
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // sets player velocity in direction of movement
        if (movement != Vector2.zero)
        {
            playerRigidbody.velocity = EightAxisUtility.direction[lastDirection] * moveSpeed;

            // play footstep audio
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        // no input
        else if (!isDashing)
        {
            // stop footstep audio
            audioSource.Stop();

            // set velocity to zero
            playerRigidbody.velocity = Vector3.zero;
        }

    }

    // public function for taking enemy damage
    public void DamageEnemy(float _damageAmount)
    {
        currentHealth -= _damageAmount;
    }

    // toggles door button prompt
    public void ToggleButtonPrompt(bool _isActive)
    {
        buttonPrompt.SetActive(_isActive);
    }

    // disables controller input
    public void DisablePlayerMovement(bool _disableMovement)
    {
        if (_disableMovement)
        {
            isInputEnabled = false;
        }
        else
        {
            isInputEnabled = true;
        }
    }

    // Checks player shooting direction and assigns the bullet spawn positions based on the 8 axis
    private void AdjustBulletSpawnPosition()
    {
        if (playerAnims.GetCurrentAnimatorStateInfo(0).IsName(playerShootNE.name)
            || playerAnims.GetCurrentAnimatorStateInfo(0).IsName(playerRunShootNE.name))
        {
            if (spriteRenderer.flipX) //NW
            {

                int[] dirArray = { 0, 1, 2 };
                int[] posArray = { 0, 1, 2 };

                CompareDirection(dirArray, NW_SpawnPositions, posArray);
            }
            else //NE
            {
                int[] dirArray = { 0, 7, 6 };
                int[] posArray = { 0, 1, 2 };

                CompareDirection(dirArray, NE_SpawnPositions, posArray);
            }
        }
        else if (playerAnims.GetCurrentAnimatorStateInfo(0).IsName(playerShootSE.name)
            || playerAnims.GetCurrentAnimatorStateInfo(0).IsName(playerRunShootSE.name))
        {
            if (spriteRenderer.flipX) // SW
            {
                int[] dirArray = { 2, 3, 4 };
                int[] posArray = { 0, 1, 2 };

                CompareDirection(dirArray, SW_SpawnPositions, posArray);
            }
            else // SE
            {
                int[] dirArray = { 4, 5, 6 };
                int[] posArray = { 0, 1, 2 };

                CompareDirection(dirArray, SE_SpawnPositions, posArray);
            }
        }
    }

    // compares shooting direction with eight axis direction and adjusts spawn position
    private void CompareDirection( int[] dir, Vector3[] spawnPos, int[] pos)
    {
        if(EightAxisUtility.direction[bubbleGun.lastDirection] == EightAxisUtility.direction[dir[0]])
        {
            bubbleGun.transform.localPosition = spawnPos[pos[0]];
        }
        else if (EightAxisUtility.direction[bubbleGun.lastDirection] == EightAxisUtility.direction[dir[1]])
        {
            bubbleGun.transform.localPosition = spawnPos[pos[1]];
        }
        else if (EightAxisUtility.direction[bubbleGun.lastDirection] == EightAxisUtility.direction[dir[2]])
        {
            bubbleGun.transform.localPosition = spawnPos[pos[2]];
        }
    }

    // player damaged visual effect (change colour of sprite)
    private IEnumerator DamagedTint()
    {
        spriteRenderer.color = damageTint;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = Color.white;
    }

    // start damage tint coroutine when player is dealt damage
    private void DamagePlayerListener(object sender, EventArgs e)
    {
        StartCoroutine(DamagedTint());
    }

    // player killed event
    public void OnPlayerKilled(object sender, EventArgs e)
    {
        PlayerKilled?.Invoke(sender, e);
    }

    private void OnDisable()
    {
        // unsubscribe listeners from events
        BaseEnemy.DamagePlayer -= DamagePlayerListener;
    }
}
