using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subclass of power ups used to implement the temporary effects from pickups
/// </summary>
[RequireComponent(typeof(Renderer))]
public class PickUp : PowerUp
{
    /// <summary>
    /// Event for when the pickup is dropped
    /// </summary>
    public static EventHandler PickupDropped;
    /// <summary>
    /// Event for when the pickup is collected
    /// </summary>
    public static EventHandler PickupCollected;

    /// <summary>
    /// Length of time for the pickup to last
    /// </summary>
    public virtual float SecondsForEffectToLast { get { return 0; } }


    /// <summary>
    /// Invokse the pickup dropped event
    /// </summary>
    private void Awake() {
        PickupDropped?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Invokes the pickup collected event, hides the object and applies it to the stat multiplier
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            PickupCollected?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
            StatMultiplier.Instance.AddPickUp(this);
        }
    }

}
