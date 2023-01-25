using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At harms length power up to increase damage the longer a bullet is in existance for
/// </summary>
public class AtHarmsLengthh : PowerUp
{
    /// <summary>
    /// increase in player bullet damage per second
    /// </summary>
    [SerializeField] private float bulletDamageIncreasePerSecond;

    /// <summary>
    /// Accessor override
    /// </summary>
    public override float BulletDamageIncreasePerSecond { get { return bulletDamageIncreasePerSecond; } }

    /// <summary>
    /// increase in enemy bullet damage per second
    /// </summary>
    [SerializeField] private float enemyBulletDamageMultiplier;

    /// <summary>
    /// Accesor override
    /// </summary>
    public override float EnemyBulletDamageMulitplier { get { return enemyBulletDamageMultiplier; } }
}
