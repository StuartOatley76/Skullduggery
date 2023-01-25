using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls all the ingame ui
/// </summary>

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject scoreFeedback;

    [Header("Pickup Info")]
    [SerializeField] private Image doubleDamage;
    [SerializeField] private Image doublePoints;
    [SerializeField] private Image invincibility;

    [Header("PowerUp Info")]
    [SerializeField] private Image powerUpImage;
    [SerializeField] private Text powerUpText;
    [SerializeField] private Color common, rare, epic, legendary;

    [Header("HiScore Info")]
    [SerializeField] private Image rosetteImage;
    [SerializeField] private Sprite firstPlaceRosette;
    [SerializeField] private Sprite secondPlaceRosette;
    [SerializeField] private Sprite thirdPlaceRosette;

    // Hiscore events
    public static event EventHandler FirstPlace;
    public static event EventHandler SecondPlace;
    public static event EventHandler ThirdPlace;

    private float score;
    public float Score { get { return score; } }

    private CharacterController player;
    private float health;
    private float totalHealth;
    private float healthDecimal;

    private float timer;
    private bool timerActive;

    void Awake()
    {
        BaseEnemy.bossDead = false;
        BaseEnemy.BossDeath += BossDeathListener;

        // reset
        score = 0;
        timer = 0;
        timerActive = true;

        // find player and initialise health bar
        player = GetComponentInParent<CharacterController>();
        if (player != null)
        {
            health = player.CurrentHealth;
            totalHealth = player.Health;
        }
    }

    void Update()
    {
        // updates player health bar
        if (player != null)
        {
            health = player.CurrentHealth;
            healthDecimal = health / totalHealth;
            healthSlider.value = healthDecimal;
        }

        // updates score text
        scoreText.text = score.ToString();

        // increases timer
        if(timerActive)
        {
            timer += Time.deltaTime;
        }
        DisplayTime();

        // sets 1st/2nd/3rd rosettes on ui
        switch (Scoreboard.instance.GetPosition(score))
        {
            // first place
            case 1:
                if (rosetteImage.sprite != firstPlaceRosette || rosetteImage.enabled == false)
                {
                    rosetteImage.enabled = true;
                    rosetteImage.sprite = firstPlaceRosette;
                    FirstPlace?.Invoke(this, EventArgs.Empty);
                }
                break;
            // second place
            case 2:
                if (rosetteImage.sprite != secondPlaceRosette || rosetteImage.enabled == false)
                {
                    rosetteImage.enabled = true;
                    rosetteImage.sprite = secondPlaceRosette;
                    SecondPlace?.Invoke(this, EventArgs.Empty);
                }
                break;
            // third place
            case 3:
                if (rosetteImage.sprite != thirdPlaceRosette || rosetteImage.enabled == false)
                {
                    rosetteImage.enabled = true;
                    rosetteImage.sprite = thirdPlaceRosette;
                    ThirdPlace?.Invoke(this, EventArgs.Empty);
                }
                break;
            default:
                rosetteImage.enabled = false;
                break;
        }

        SetPickupUI();
        SetPowerUpUI();
    }

    // displays pickup ui when pickup is in effect
    private void SetPickupUI()
    {
        // double score points pickup
        if (StatMultiplier.Instance.pickupsInEffect.Sum(p => p.ScoreMultiplier) > 0)
        {
            doublePoints.enabled = true;
        }
        else
        {
            doublePoints.enabled = false;
        }

        // double damage pickup
        if (StatMultiplier.Instance.pickupsInEffect.Sum(p => p.StandardEnemyDamageMultiplier) > 0)
        {
            doubleDamage.enabled = true;
        }
        else
        {
            doubleDamage.enabled = false;
        }

        // invincibility pickup
        if (StatMultiplier.Instance.pickupsInEffect.Any(p => p.Invincibility))
        {
            invincibility.enabled = true;
        }
        else
        {
            invincibility.enabled = false;
        }
    }

    // displays the powerup that is active
    private void SetPowerUpUI()
    {
        // gets active power up
        var powerup = StatMultiplier.Instance.CurrentPowerUp;

        // sets icon colour based on powerup rarity
        switch (powerup.Rarity)
        {
            case Rarity.Common:
                powerUpImage.color = common;
                break;
            case Rarity.Rare:
                powerUpImage.color = rare;
                break;
            case Rarity.Epic:
                powerUpImage.color = epic;
                break;
            case Rarity.Legendary:
                powerUpImage.color = legendary;
                break;
            default:
                break;
        }

        // displays power up name
        if (powerup.GetComponent<NoClaimsBonus>())
        {
            powerUpText.text = "No Claims Bonus";
        }
        else if (powerup.GetComponent<AtHarmsLengthh>())
        {
            powerUpText.text = "At Harm's Length";
        }
        else if (powerup.GetComponent<SurvivorsGuilt>())
        {
            powerUpText.text = "Survivors Guilt";
        }
        else
        {
            powerUpText.text = "No Power Up";
        }
    }

    // public function to increase score
    public void UpdateScore(float scoreValue)
    {
        score += scoreValue;
    }

    // displays timer in 00:00 format
    private void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void BossDeathListener(object sender, EventArgs e)
    {
        timerActive = false;
        float maxTime = 7 * 60;
        if( !(maxTime - timer <= 0) )
        {
            var points = Mathf.FloorToInt(((maxTime - timer) / 30) * 100);
            score += points;

            // create score feedback
            var feedback = Instantiate(scoreFeedback, player.transform.position, Quaternion.identity);
            feedback.GetComponentInChildren<Text>().text = points.ToString();
        }
    }

    private void OnDisable()
    {
        BaseEnemy.BossDeath -= BossDeathListener;
    }
}
