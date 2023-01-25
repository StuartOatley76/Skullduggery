using UnityEngine;

/// <summary>
/// pickup to increase damage output 
/// </summary>
public class DoubleDamagePickup : PickUp
{
    /// <summary>
    /// How much the damage is increased by
    /// </summary>
    [SerializeField] private float damageMultiplierAddition;
    /// <summary>
    /// How long the pick up lasts for
    /// </summary>
    [SerializeField] private float secondsToLastFor;

    //Overrides to implement the above
    public override float StandardEnemyDamageMultiplier { get { return damageMultiplierAddition; } }
    public override float DamageToBossMultiplier { get { return damageMultiplierAddition; } }
    public override float SecondsForEffectToLast { get { return secondsToLastFor; } }
}
