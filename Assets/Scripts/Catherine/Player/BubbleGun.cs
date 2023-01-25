using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Bubble gun behaviour - stores all bubble gun data and receives shooting input from the controller
/// </summary>

public class BubbleGun : MonoBehaviour
{
    public static event EventHandler PlayerAttack;

    [Header("Bubble Info")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private int bubbleAmount;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bubbleSpeed = 30f;

    [Header("Special Info")]
    [SerializeField] private float scaleIncreaseInterval;
    [SerializeField] private float specialScaleIncrease;
    [SerializeField] private float maxSpecialScale;

    private GameObject bubbleSpecial;
    private float specialHeldTimer;
    private bool specialHeld = false;

    [HideInInspector]
    public Vector2 shootingDirection;
    [HideInInspector]
    public int lastDirection;
    private float previousShot;

    private CharacterController player;

    private void Start()
    {
        // set up variables
        player = GetComponentInParent<CharacterController>();
        previousShot = fireRate;
    }

    private void Update()
    {
        // checks if paused
        if( Time.timeScale > 0 )
        {
            // receives shooting input
            shootingDirection = new Vector2(Input.GetAxisRaw("Gamepad_Horizontal_R"), Input.GetAxisRaw("Gamepad_Vertical_R"));
            lastDirection = EightAxisUtility.EightAxisDirection(shootingDirection);

            // sets shooting direction to zero if in controller deadzone
            if (shootingDirection.magnitude < player.inputDeadzone)
            {
                shootingDirection = Vector2.zero;
            }

            // fire bullets when shooting and not dashing
            if (!player.isDashing && shootingDirection != Vector2.zero)
            {
                if (!specialHeld && Time.time > fireRate + previousShot)
                {
                    Fire();
                }
            }
            else 
            {
                // disable shooting
                player.isShooting = false;

                // set player animator bool
                player.playerAnims.SetBool("isShooting", player.isShooting);
            }

            // DISABLED - SPECIAL ATTACK
            //if (Input.GetAxisRaw("Gamepad_Fire2") > 0 && !specialHeld && !player.isDashing)
            //{
            //    SpecialCharge();
            //}
            //else if (Input.GetAxisRaw("Gamepad_Fire2") == 0 && specialHeld)
            //{
            //    SpecialAttack();
            //}
            //
            //if (specialHeld && bubbleSpecial != null)
            //{
            //    specialHeldTimer += Time.deltaTime;
            //    if (specialHeldTimer > scaleIncreaseInterval)
            //    {
            //        specialHeldTimer -= scaleIncreaseInterval;
            //        if (!(bubbleSpecial.transform.localScale == Vector3.one * maxSpecialScale))
            //        {
            //            bubbleSpecial.transform.localScale = bubbleSpecial.transform.localScale + (Vector3.one * specialScaleIncrease);
            //        }
            //    }
            //}
        }
    }

    private void Fire()
    {
        // player attack event
        OnPlayerAttack(this, EventArgs.Empty);
        player.isShooting = true;

        // set player animator bool
        player.playerAnims.SetBool("isShooting", player.isShooting);

        // spawns bubbles and sets shooting direction in a spread
        for (int i = 0; i < bubbleAmount; i++)
        {
            GameObject bubble = Instantiate(bubblePrefab, transform.position, transform.rotation);
            Vector3 dir;
            Vector3 tempDir;

            switch(i)
            {
                case 0:
                    if(lastDirection == 0) // 0th axis
                    {
                        tempDir = AdjustBulletDirection(lastDirection + 1);
                    }
                    else if(lastDirection == 7) // 7th axis
                    {
                        tempDir = AdjustBulletDirection(lastDirection - 7);
                    }
                    else
                    {
                        tempDir = AdjustBulletDirection(lastDirection - 1);
                    }
                    // adds vectors together to get vector between for shooting direction
                    dir = EightAxisUtility.direction[lastDirection] + tempDir;
                    // sets as velocity
                    bubble.GetComponent<Rigidbody>().velocity = dir.normalized * bubbleSpeed;                   
                    break;

                case 1:
                    // shooting direction
                    dir = EightAxisUtility.direction[lastDirection];
                    // sets as velocity
                    bubble.GetComponent<Rigidbody>().velocity = dir.normalized * bubbleSpeed;
                    break;

                case 2:
                    if (lastDirection == 7) // 7th axis
                    {
                        tempDir = AdjustBulletDirection(lastDirection - 1);
                    }
                    else if(lastDirection == 0) // 0th axis
                    {
                        tempDir = AdjustBulletDirection(lastDirection + 7); 
                    }
                    else
                    {
                        tempDir = AdjustBulletDirection(lastDirection + 1);
                    }

                    // adds vectors together to get vector between for new shooting direction
                    dir = EightAxisUtility.direction[lastDirection] + tempDir;
                    // sets as velocity
                    bubble.GetComponent<Rigidbody>().velocity = dir.normalized * bubbleSpeed;                   
                    break;
            }
        }        
        previousShot = Time.time;
    }

    // adds shooting direction and previous/next direction in 8 axis together to get the vector between, then divides to get a smaller spread
    private Vector3 AdjustBulletDirection(int dir)
    {
        return (EightAxisUtility.direction[lastDirection] + EightAxisUtility.direction[dir]) / 5;
    }

    // player attack event
    public void OnPlayerAttack(object sender, EventArgs e)
    {
        PlayerAttack?.Invoke(sender, e);
    }

    //DISABLED - SPECIAL ATTACK
    // charges special and disables movement
    //private void SpecialCharge()
    //{
    //    player.DisablePlayerMovement(true);
    //    specialHeld = true;
    //    bubbleSpecial = Instantiate(bubblePrefab, transform.position, transform.rotation);
    //}
    //
    //// fires the special attack
    //private void SpecialAttack()
    //{
    //    player.DisablePlayerMovement(false);
    //    specialHeld = false;
    //    if(bubbleSpecial != null)
    //    {
    //        bubbleSpecial.GetComponent<Bullet>().velocity = EightAxisUtility.direction[lastDirection] * bubbleSpeed;
    //    }
    //}
}
