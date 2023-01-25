using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls fistpunch trap, inherits from BaseTrap
/// </summary>

public class FistPunchTrap : BaseTrap
{
    public override void ActivateTrap()
    {
        base.ActivateTrap();
        // play activated trap animation
        animator.Play(trapAnim.name);
    }
}
