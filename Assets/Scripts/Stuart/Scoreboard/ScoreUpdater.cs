using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to update the visual side of the high score board
/// </summary>
public class ScoreUpdater : MonoBehaviour
{
    /// <summary>
    /// List of text objects for to display the names
    /// </summary>
    [SerializeField] private List<Text> names;

    /// <summary>
    /// List of text objects do display the scores
    /// </summary>
    [SerializeField] private List<Text> scores;

    /// <summary>
    /// Name entry system
    /// </summary>
    [SerializeField] private NameEntry entry;

    /// <summary>
    /// initialises
    /// </summary>
    private void Start() {
        UpdateScores();
    }

    /// <summary>
    /// Updates the text objects with the latest values in Scoreboard
    /// </summary>
    public void UpdateScores() {
        List<KeyValuePair<string, float>> highscores = Scoreboard.instance.scores;
        for (int i = 0; i < names.Count && i < scores.Count  && i < highscores.Count; i++) {
            names[i].text = highscores[i].Key;
            scores[i].text = highscores[i].Value.ToString();
        }
    }

    /// <summary>
    /// allows exiting the scoreboard by pressing B
    /// </summary>
    private void Update() {
        if(entry.gameObject.activeSelf == false && Input.GetButtonDown("Gamepad_B")) {
            SceneManager.LoadScene("Menu");
        }
    }
}
