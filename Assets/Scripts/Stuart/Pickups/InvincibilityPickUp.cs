using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pick up to grant invincibility
/// </summary>
public class InvincibilityPickUp : PickUp
{
    /// <summary>
    /// How long the pickup lasts for
    /// </summary>
    [SerializeField] private float secondsToLastFor;

    //Overrides to implement pickup
    public override float SecondsForEffectToLast { get { return secondsToLastFor; } }
    public override bool Invincibility { get { return true; } }
}
