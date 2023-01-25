using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum to represent rarity with values as chance of getting them
/// Number picked from 100 and player gets whichever it is <= to
/// /// </summary>
public enum Rarity {
    Common = 100,
    Rare = 40,
    Epic = 15,
    Legendary = 5
}

/// <summary>
/// Base class for Power ups
/// </summary>
public class PowerUp : MonoBehaviour
{
    /// <summary>
    /// Sprite used for unselected in the menu
    /// </summary>
    [SerializeField] private Sprite unselected;
    /// <summary>
    /// sprite used for selected in the menu
    /// </summary>
    [SerializeField] private Sprite selected;
    /// <summary>
    /// Rarity of this power up
    /// </summary>
    [SerializeField] private Rarity rarity;

    //Accessors

    public Rarity Rarity { get { return rarity; } }
    public Sprite UnselectedIcon { get { return unselected; } }
    public Sprite SelectedIcon { get { return selected; } }

    //Virtual accessors to be overridden to give effects
    public virtual float StandardEnemyDamageMultiplier { get { return 0; } }

    public virtual float BossHealthMultiplier { get { return 0; } }

    public virtual float DamageToBossMultiplier { get { return 0; } }

    public virtual float BulletDamageIncreasePerSecond { get { return 0; } }

    public virtual float EnemyBulletDamageMulitplier { get { return 0; } }

    public virtual bool Invincibility { get { return false; } }

    public virtual float ScoreMultiplier { get { return 0; } }
}
