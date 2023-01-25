using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine Behaviour for player bullet animations
/// </summary>

public class DestroyBubble : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // destroy bullet after death anim has played
        Destroy(animator.gameObject.transform.parent.gameObject);
    }
}
