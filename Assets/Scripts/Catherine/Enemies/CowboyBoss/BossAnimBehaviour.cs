using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine Behaviour for Cowboy Boss
/// </summary>

public class BossAnimBehaviour : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set is attacking to false when attack anim ends
        //animator.gameObject.GetComponentInParent<CowboyBoss>().isAttacking = false;
    }
}
