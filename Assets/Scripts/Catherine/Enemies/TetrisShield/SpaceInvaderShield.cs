using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
///  Tetris block shield behaviour
/// </summary>

public class SpaceInvaderShield : BaseEnemy
{
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;

    private Vector3 target;
    private float timer;

    private void OnEnable()
    {
        // set wander timer
        timer = wanderTimer;
    }

    private void Update()
    {
        // increase timer
        timer += Time.deltaTime;

        if (currentHealth <= 0) 
        {
            // enemy killed event
            OnEnemyKilled(this, EventArgs.Empty);
        }
        // wander after time
        else if(timer >= wanderTimer)
        {
            // get random target
            target = RandomTarget(transform.position, wanderRadius);
            // set target as nevmesh destination
            enemy.SetDestination(target);

            // reset timer
            timer = 0;
        }
    }
}