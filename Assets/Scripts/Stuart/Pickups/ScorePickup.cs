using UnityEngine;

/// <summary>
/// Pickup to increase the score the player recieves
/// </summary>
public class ScorePickup : PickUp
{
    /// <summary>
    /// The increase in the scores
    /// </summary>
    [SerializeField] private float scoreMultiplierAddition;
    /// <summary>
    /// How long it lasts for
    /// </summary>
    [SerializeField] private float secondsToLastFor;

    // Overrides to implement the above
    public override float SecondsForEffectToLast { get { return secondsToLastFor; } }
    public override float ScoreMultiplier { get { return scoreMultiplierAddition; } }
}
