using UnityEngine;
using System;

/// <summary>
/// State Machine Behaviour for frog puddle spawn
/// </summary>

public class PuddlePlacement : StateMachineBehaviour
{
    [SerializeField] private GameObject puddle;
    [SerializeField] private GameObject splashEffect;
    [SerializeField] private Vector3 particleOffset;

    // puddle events
    public static event EventHandler PuddleSplash;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var go = animator.gameObject.GetComponentInParent<GringoHopps>();

        // reset frog navmesh path
        go.enemy.ResetPath();
        // set velocity to zero
        go.enemy.velocity = Vector3.zero;
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // reenable damage anim
        go.canPlayDamageAnim = true;

        // puddle splash event
        PuddleSplash?.Invoke(this, EventArgs.Empty);

        // spawn puddle object and puddle splash effect
        Instantiate(puddle, animator.gameObject.transform.position, animator.gameObject.transform.rotation);
        Instantiate(splashEffect, animator.gameObject.transform.position += particleOffset, splashEffect.transform.rotation);
    }
}
