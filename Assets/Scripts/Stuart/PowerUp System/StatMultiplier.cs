using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle multipliers and effects on the character and enemies stats
/// </summary>
public class StatMultiplier : MonoBehaviour
{
    //Singleton instance
    public static StatMultiplier Instance { get; private set; }

    /// <summary>
    /// sets up instance and default power up (no effect on stats
    /// </summary>
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
        CurrentPowerUp = Instantiate(defaultPowerUp);
    }

    /// <summary>
    /// The character controller from the player
    /// </summary>
    private CharacterController character;
    public CharacterController Character {
        get { return character; }
        set {
            character = value;
        }
    }

    /// <summary>
    /// The prefab for the default powerup
    /// </summary>
    [SerializeField] private PowerUp defaultPowerUp;

    /// <summary>
    /// The power up currently in effect
    /// </summary>
    private PowerUp currentPowerUp;

    /// <summary>
    /// List of pick ups currently in effect
    /// </summary>
    public List<PickUp> pickupsInEffect = new List<PickUp>();
    public PowerUp CurrentPowerUp { get { return currentPowerUp; }
        set {
            currentPowerUp = value;
        } 
    }

    /// <summary>
    /// Icon of power up currently in effect
    /// </summary>
    public Sprite Icon { get { return CurrentPowerUp.SelectedIcon; } }

    /// <summary>
    /// Multiplier for damage against standard enemies
    /// </summary>
    public float StandardEnemyDamageMultiplier { get { 
            return 1 + CurrentPowerUp.StandardEnemyDamageMultiplier + pickupsInEffect.Sum(p => p.StandardEnemyDamageMultiplier); 
        } 
    }
    /// <summary>
    /// Multiplier for the boss' health
    /// </summary>
    public float BossHealthMultiplier { get { 
            return 1 + CurrentPowerUp.BossHealthMultiplier + pickupsInEffect.Sum(p => p.BossHealthMultiplier); 
        } 
    }

    /// <summary>
    /// multiplier for damage against the boss
    /// </summary>
    public float DamageToBossMultiplier { get {
            return 1 + currentPowerUp.DamageToBossMultiplier + pickupsInEffect.Sum(p => p.DamageToBossMultiplier);
        } 
    }

    //Note - as this is culmative over time it's not a percentage. 
    //It'll need to be bullet base damage * this * seconds bullet has existed + bullet base damage
    /// <summary>
    /// increase in damage caused by player bullets per second they exist
    /// </summary>
    public float BulletDamageIncreasePerSecond { get {
            return currentPowerUp.BulletDamageIncreasePerSecond + pickupsInEffect.Sum(p => p.BulletDamageIncreasePerSecond);
        } 
    }

    /// <summary>
    /// Multiplier for damage caused by enemy bullets
    /// </summary>
    public float EnemyBulletDamageMultiplier { get {
            return 1 + currentPowerUp.EnemyBulletDamageMulitplier + pickupsInEffect.Sum(p => p.EnemyBulletDamageMulitplier);
        } 
    }

    /// <summary>
    /// Mulitplier for score recieved
    /// </summary>
    public float ScoreMultiplier { get {
            return 1 + currentPowerUp.ScoreMultiplier + pickupsInEffect.Sum(p => p.ScoreMultiplier);
        }
    }


    /// <summary>
    /// Whether the player is invincible
    /// </summary>
    public bool Invincibility { get {
            return currentPowerUp.Invincibility || pickupsInEffect.Any(p => p.Invincibility);
        } 
    } 

    /// <summary>
    /// Adds a pickup to the list and sets the timer going for it to be removed
    /// </summary>
    /// <param name="pickUp"></param>
    public void AddPickUp(PickUp pickUp) {
        pickupsInEffect.Add(pickUp);
        if(pickUp.SecondsForEffectToLast >= 0) {
            StartCoroutine(RemovePickup(pickUp));
        }
    }

    /// <summary>
    /// Removes the pick up after the time has elapsed
    /// </summary>
    /// <param name="pickUp"></param>
    /// <returns></returns>
    private IEnumerator RemovePickup(PickUp pickUp) {
        yield return new WaitForSeconds(pickUp.SecondsForEffectToLast);
        pickupsInEffect.Remove(pickUp);
        Destroy(pickUp.gameObject);
    }
}
