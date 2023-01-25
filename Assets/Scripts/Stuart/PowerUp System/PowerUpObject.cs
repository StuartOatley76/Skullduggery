using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour for on a powerup's gameobject
/// </summary>
public class PowerUpObject : MonoBehaviour
{
    /// <summary>
    /// Event for when the power up is dropped
    /// </summary>
    public static EventHandler PowerUpDropped;
    /// <summary>
    /// event for when the power up is picked up
    /// </summary>
    public static EventHandler PowerUpPickedUp;
    /// <summary>
    /// The power up selector that chooses the powerup
    /// </summary>
    private PowerUpSelector selector;

    /// <summary>
    /// Finds the selector and triggers the dropped event
    /// </summary>
    private void Awake() {
        selector = Resources.FindObjectsOfTypeAll<PowerUpSelector>()[0];
        PowerUpDropped?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Activates the selector and invokes the picked up event
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            selector.gameObject.SetActive(true);
            PowerUpPickedUp?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Destroys the gameobject
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        Destroy(gameObject);
    }
}
