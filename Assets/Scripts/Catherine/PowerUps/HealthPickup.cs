using UnityEngine;
using System;

/// <summary>
/// Controls the health pickup
/// </summary>

public class HealthPickup : PickUp
{
    // heath increase value
    [SerializeField] private float healthIncrease;

    // activates on player collision
    private void OnCollisionEnter(Collision collision)
    {       
        var player = collision.gameObject.GetComponent<CharacterController>();
        if (player)
        {
            // pickup collected event
            PickupCollected?.Invoke(this, EventArgs.Empty);

            // increases player health
            player.CurrentHealth += healthIncrease;

            // don't go above total health
            if(player.CurrentHealth > player.Health)
            {
                player.CurrentHealth = player.Health;
            }

            // Destroys pickup in scene
            Destroy(gameObject);
        }
    }
}
