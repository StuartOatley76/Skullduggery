//Written by Toby Harris-St John, with alterations by Stuart Oatley and Catherine Burns.

using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DestructableObject : MonoBehaviour
{
    //Events - Set up by Stuart & Catherine to work with the games sound manager and play the correct sounds when destructables are hit or destroyed.
    public static EventHandler DestructableBoxHit;
    public static EventHandler DestructableBoxDestroyed;
    public static EventHandler DestructablePlushHit;
    public static EventHandler DestructablePlushDestroyed;

    [SerializeField] private GameObject scoreFeedback;

    [SerializeField] private bool isPlush; //Plushie destructables have unique sound. Set up by Catherine, so that different sounds could be played for Plushie destructables.
    [SerializeField] private Transform player;
    [SerializeField] private InGameUI gameUI;
    [SerializeField] private float destroyScore; //Score added when destroyed. Randomised between min/max values.
    [SerializeField] private float destroyScoreMax; 
    [SerializeField] private float destroyScoreMin;
    [SerializeField] private int destructableHP; //How many hits to destroy the object.
    [SerializeField] private int damageThreshold; //Threshold before damage is shown. Only applies if showDamage is true.
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private bool showDamage; //Enables/disables material swapping to simulate damage. Only used on plushie destructables.
    [SerializeField] private Material baseMat;
    [SerializeField] private Material damaged1Mat;
    [SerializeField] private Material damaged2Mat;
    [SerializeField] private Animator animator;

    private bool isHit = false;

    public GameObject deathParticle; //Particle effect to play when destroyed.
    public float particleYPos; //Position of particle effect.

    public void Awake()
    {
        if (showDamage) //Get the meshrenderer (only required when damage is enabled).
        {
            meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        animator = gameObject.GetComponentInChildren<Animator>();

        damageThreshold = destructableHP / 2; //Set damage threshold to half HP.

        destroyScore = Mathf.RoundToInt(UnityEngine.Random.Range(destroyScoreMin, destroyScoreMax) * ComboSystem.comboMultiplier * StatMultiplier.Instance.ScoreMultiplier); //Set score to random int between min/max values.

        if (baseMat != null)
        {
            meshRenderer.material = baseMat;
        }

        StartCoroutine(WaitForPlayerToSpawn()); //Wait for player to spawn before getting player references.
    }

    IEnumerator WaitForPlayerToSpawn()
    {
        yield return new WaitUntil(() => PlayerSpawner.instance.Player != null); //Set up by Stuart Oatley to work with his room generation code.

        player = PlayerSpawner.instance.Player.transform;
        gameUI = player.GetComponentInChildren<InGameUI>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit) //If not already hit. Prevents bullets hitting multiple times.
        {
            if (collision.gameObject.tag == "Bullet")
            {
                isHit = true;

                if (animator != null)
                {
                    animator.SetTrigger("Hit"); //Play hit animation. Wobbles object.
                }

                if (destructableHP <= 0) //If HP is all gone.
                {
                    if (deathParticle != null)
                    {
                        GameObject death = Instantiate(deathParticle, transform.position, transform.rotation); //Create the death particle effect.
                        death.transform.position = new Vector3(death.transform.position.x, particleYPos, death.transform.position.z);
                    }
                    if(isPlush) //If plush destructable.
                    {
                        DestructablePlushDestroyed?.Invoke(this, EventArgs.Empty); //Trigger the PlushDestroyed event.
                    }
                    else
                    {
                        DestructableBoxDestroyed?.Invoke(this, EventArgs.Empty); //Trigger the BoxDestroyed event.
                    }

                    // catherine - create score feedback
                    var feedback = Instantiate(scoreFeedback, transform.position, Quaternion.identity);
                    feedback.GetComponentInChildren<Text>().text = destroyScore.ToString();

                    gameUI.UpdateScore(destroyScore); //Add score value to player's score
                    Destroy(gameObject);
                }
                else
                {
                    if(isPlush) //If plush destructable.
                    {
                        DestructablePlushHit?.Invoke(this, EventArgs.Empty); //Trigger PlushHit event.
                    }
                    else
                    {
                        DestructableBoxHit?.Invoke(this, EventArgs.Empty); //Trigger BoxHit event.
                    }
                    destructableHP += -1; //Reduce HP.
                    if (showDamage) //If you want to show damage,
                    {
                        UpdateMaterial(); //swap the material to correct damaged material.
                    }
                    isHit = false; //Reset isHit.
                }
            }
        }

    }

    private void UpdateMaterial() //Set material to relevant material for damage amount based on HP and damageThreshold.
    {
        if (destructableHP <= 1)
        {
            if (damaged2Mat != null)
            {
                meshRenderer.material = damaged2Mat;
            }
        }
        else if (destructableHP <= damageThreshold)
        {
            if (damaged1Mat != null)
            {
                meshRenderer.material = damaged1Mat;
            }
        }
    }
}
