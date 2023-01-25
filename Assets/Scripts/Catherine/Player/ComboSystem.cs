using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;
using UnityEngine.UI;

/// <summary>
/// Controls combo system
/// </summary>

public class ComboSystem : MonoBehaviour
{
    public static float comboMultiplier;

    [SerializeField] private Animator comboAnimator;
    [SerializeField] private Slider comboSlider;

    [Header("Combo Animations")]
    [SerializeField] private AnimationClip lowComboAnim;
    [SerializeField] private AnimationClip midComboAnim;
    [SerializeField] private AnimationClip highComboAnim;

    [Header("Combo Thresholds")]
    [SerializeField] private float midComboValue;
    [SerializeField] private float highComboValue;
    [SerializeField] private float maxComboValue;

    [Header("Combo Stats")]
    [SerializeField] private float comboDecreaseValue;
    [SerializeField] private float comboIncreaseValue;
    [SerializeField] private float playerHitPenaltyMultiplier;

    [Header("Combo VFX settings")]
    [SerializeField] private VisualEffect comboVFX;
    [SerializeField] private float comboVFXMaxRadius;
    [SerializeField] private float comboVFXMinRadius;

    private float comboCurrentValue;

    // combo events
    public static EventHandler LowCombo;
    public static EventHandler MidCombo;
    public static EventHandler HighCombo;

    private void Start()
    {
        // reset values
        comboVFX.Stop();
        comboCurrentValue = 0f;
        comboMultiplier = 1f;

        // subscribe to damage events
        BaseEnemy.DamagePlayer += DamagePlayerListener;
        Bullet.DamageEnemy += DamageEnemyListener;
    }

    private void Update()
    {      
        if ( comboCurrentValue > 0 )
        {
            // decrease combo value over time
            comboCurrentValue -= comboDecreaseValue * Time.deltaTime;

            // don't go below minimum value
            if ( comboCurrentValue < 0 )
            {
                comboCurrentValue = 0f;
            }
        }

        // update combo slider
        comboSlider.value = comboCurrentValue / maxComboValue;

        // sets combo multipliers
        if ( comboCurrentValue >= highComboValue ) // high
        {
            // play combo vfx 
            comboVFX.Play();

            if ( comboMultiplier != 3 )
            {
                comboMultiplier = 3f;
                // high combo event
                HighCombo?.Invoke(this, EventArgs.Empty);
            }

            // update combo anim
            comboAnimator.Play(highComboAnim.name);
        }
        else if (comboCurrentValue >= midComboValue) // mid
        {
            // disable combo vfx
            comboVFX.Stop();

            if (comboMultiplier != 2 )
            {
                comboMultiplier = 2f;
                // mid combo event
                MidCombo?.Invoke(this, EventArgs.Empty);
            }

            // update combo anim
            comboAnimator.Play(midComboAnim.name);
        }
        else // low
        {
            // disable combo vfx
            comboVFX.Stop();

            if ( comboMultiplier != 1 )
            {
                comboMultiplier = 1f;
                // low combo event
                LowCombo?.Invoke(this, EventArgs.Empty);
            }

            // update combo anim
            comboAnimator.Play(lowComboAnim.name);
        }

        // resets combo when room is cleared
        if ( comboCurrentValue == 0 || BaseEnemy.enemyCounter <= 0 )
        {
            comboCurrentValue = 0f;
            comboVFX.Stop();
        }

        AdjustComboVFX();
    }

    private void AdjustComboVFX()
    {
        // adjusts the radius of the vfx based on the combo value (greater effect the higher the value)
        if( comboCurrentValue > 0 )
        {
            var radius = comboVFXMinRadius - (comboCurrentValue * ((comboVFXMinRadius - comboVFXMaxRadius) / maxComboValue));
            comboVFX.SetFloat("radius", radius);
        }
    }

    // increase combo value when hitting enemies
    private void DamageEnemyListener(object sender, EventArgs e)
    {        
        comboCurrentValue += comboIncreaseValue;
        // don't go above maximum value
        if (comboCurrentValue > maxComboValue)
        {
            comboCurrentValue = maxComboValue;
        }
    }

    // decrease combo value when taking damage
    private void DamagePlayerListener(object sender, EventArgs e)
    {
        comboCurrentValue -= comboIncreaseValue * playerHitPenaltyMultiplier;
        // don't go below minimum value
        if (comboCurrentValue < 0f)
        {
            comboCurrentValue = 0f;
        }
    }

    private void OnDisable()
    {
        // unsubscribe listeners from events
        BaseEnemy.DamagePlayer -= DamagePlayerListener;
        Bullet.DamageEnemy -= DamageEnemyListener;
    }
}
