using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Survivors Guilt powerup which increases player damage output, but increases boss health
/// </summary>

public class SurvivorsGuilt : PowerUp
{
    // player damage multiplier
    [SerializeField] private float damageMultiplier;
    // boss health multiplier
    [SerializeField] private float bossHealthMultiplier;

    // Accessor overrides
    public override float StandardEnemyDamageMultiplier { get { return damageMultiplier; } }
    public override float BossHealthMultiplier { get { return bossHealthMultiplier; } }
}
