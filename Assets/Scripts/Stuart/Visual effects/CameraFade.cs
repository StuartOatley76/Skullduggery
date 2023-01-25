using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle fading in and out
/// </summary>
public class CameraFade : MonoBehaviour
{
    /// <summary>
    /// speed of fade
    /// </summary>
    public float fadeSpeed { get; set; } = 1f;

    /// <summary>
    /// Color to fade to and from
    /// </summary>
    public Color fadeColour { get; set; } = Color.black;

    /// <summary>
    /// Whether the fade is active
    /// </summary>
    public bool Fading { get; set; }

    /// <summary>
    /// Whether the camera is faded out
    /// </summary>
    public bool IsFaded {
        get { return alpha >= 1; }
    }

    /// <summary>
    /// Whether the camera should start faded on scene load
    /// </summary>
    [SerializeField] private bool startFaded;

    /// <summary>
    /// The alpha value of the texture used to implement the fade
    /// </summary>
    private float alpha = 0f;

    /// <summary>
    /// the texture used to implement the fade
    /// </summary>
    private Texture2D texture;

    /// <summary>
    /// How long the fade has been running
    /// </summary>
    private float time = 0f;

    /// <summary>
    /// The alpha value at the start of the fade
    /// </summary>
    private int startAlpha;

    /// <summary>
    /// The target alpha value
    /// </summary>
    private int endAlpha;

    /// <summary>
    /// initialisation
    /// </summary>
    private void Awake() {
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, alpha));
        texture.Apply();
        alpha = startFaded ? 1 : 0;
        CharacterController.PlayerKilled += FadeToMenu;
    }

    /// <summary>
    /// Clean up
    /// </summary>
    private void OnDisable() {
        CharacterController.PlayerKilled -= FadeToMenu;
    }

    /// <summary>
    /// Delegate that starts fade
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FadeToMenu(object sender, EventArgs e) {
        StartCoroutine(TransitionToMenu());
    }

    /// <summary>
    /// uses fade to transition to the next scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionToMenu() {
        ToggleFade();
        while (Fading) {
            yield return null;
        }
        if (Scoreboard.instance.NewHighScore) {
            SceneManager.LoadScene("TrophyScene");
        } else {
            SceneManager.LoadScene("Menu");
        }
    }

    /// <summary>
    /// Sets values to switch fade to in or out
    /// </summary>
    public void ToggleFade() {
 
        Fading = true;
        CharacterController characterController = FindObjectOfType<CharacterController>();
        if (characterController) {
            characterController.DisablePlayerMovement(Fading);
        }
        if (alpha >= 1) {
            alpha = 1f;
            time = 0f;
            startAlpha = 1;
            endAlpha = 0;

        } else {
            alpha = 0f;
            time = 0f;
            startAlpha = 0;
            endAlpha = 1;
        }
    }

    /// <summary>
    /// Implenemts the fade
    /// </summary>
    private void OnGUI() {
        if(alpha > 0) {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        }
        if(Fading) {
            time += Time.deltaTime * fadeSpeed;
            alpha = Mathf.Lerp(startAlpha, endAlpha, time);
            texture.SetPixel(0, 0, new Color(fadeColour.r, fadeColour.g, fadeColour.b, alpha));
            texture.Apply();
            if(alpha <= 0 || alpha >= 1) {
                Fading = false;
                CharacterController characterController = FindObjectOfType<CharacterController>();
                if (characterController) {
                    characterController.DisablePlayerMovement(Fading);
                }
            }
        }
    }

    /// <summary>
    /// Clean up
    /// </summary>
    private void OnDestroy() {
        CharacterController.PlayerKilled -= FadeToMenu;
    }
}
