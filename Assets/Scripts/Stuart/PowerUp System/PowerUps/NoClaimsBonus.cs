using System;
using UnityEngine;

/// <summary>
/// No Claims bonus power up that gives a boost to how much damage the player does 
/// based on how many enemies they kill before taking damage
/// </summary>
public class NoClaimsBonus : PowerUp
{
    /// <summary>
    /// Damage multiplier applied to give handicap
    /// </summary>
    [SerializeField] private float baseMultiplier = 0.25f;

    /// <summary>
    /// increase in damage percentage for each enemy killed
    /// </summary>
    [SerializeField] private float increaseAmount = 0.05f;

    /// <summary>
    /// Accessor override
    /// </summary>
    public override float StandardEnemyDamageMultiplier { get { return currentMultiplier; } }

    /// <summary>
    /// current damage multiplier
    /// </summary>
    private float currentMultiplier;

    /// <summary>
    /// sets Current and connects to relevent events
    /// </summary>
    private void Awake() {
        currentMultiplier = baseMultiplier;
        BaseEnemy.EnemyKilled += IncreaseMultiplier;
        BaseEnemy.DamagePlayer += ResetMultiplier;
    }

    /// <summary>
    /// Increases the multiplier
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IncreaseMultiplier(object sender, EventArgs e) {
        currentMultiplier += increaseAmount;
    }

    /// <summary>
    /// Sets the multiplier back to it's initial value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetMultiplier(object sender, EventArgs e) {
        currentMultiplier = baseMultiplier;
    }

    /// <summary>
    /// Disconnects from events
    /// </summary>
    private void OnDestroy() {
        BaseEnemy.EnemyKilled -= IncreaseMultiplier;
        BaseEnemy.DamagePlayer -= ResetMultiplier;
    }
}
