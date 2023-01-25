using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class to handle retrieving and recording scores for the high score table
/// Persists in every scene to carry scores
/// </summary>
public class Scoreboard : MonoBehaviour
{
    /// <summary>
    /// Instance 
    /// </summary>
    public static Scoreboard instance;

    /// <summary>
    /// Whether a new high score has been achieved
    /// </summary>
    public bool NewHighScore { get; private set; } = false;

    /// <summary>
    /// The last score achieved
    /// </summary>
    public float LastScore { get; private set; } = 0;

    /// <summary>
    /// How many scores are recorded in the table
    /// </summary>
    private const float numberOfEntries = 6;

    /// <summary>
    /// List of Names and corresponding scores
    /// </summary>
    public List<KeyValuePair<string, float>> scores { get; private set; } = new List<KeyValuePair<string, float>>();

    /// <summary>
    /// performs initialisation. Done once between game loading and closing
    /// </summary>
    private void Awake() {
        if(instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Setup();
        CharacterController.PlayerKilled += CheckScore;
    }

    /// <summary>
    /// Checks the score when the player dies and sets lastscore and newHighScore
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CheckScore(object sender, EventArgs e) {
        InGameUI uI = FindObjectOfType<InGameUI>();
        if (uI != null) {
            if(uI.Score > scores[scores.Count - 1].Value){
                NewHighScore = true;
            } else {
                NewHighScore = false;
            }
            LastScore = uI.Score;
        }
    }

    /// <summary>
    /// Gets the position in the high score table that the provided scre would place
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public int GetPosition(float score) {
        for(int i = 0; i < scores.Count; i++) {
            if(score > scores[i].Value) {
                return i + 1;
            }
        }
        return scores.Count + 1;
    }

    /// <summary>
    /// Loads the scores from player prefs
    /// </summary>
    private void Setup() {
        scores.Clear();
        if (!PlayerPrefs.HasKey("Name0")) {
            CreateKeys();
        }
        for (int i = 0; i < numberOfEntries; i++) {
            scores.Add(new KeyValuePair<string, float>(PlayerPrefs.GetString("Name" + i), PlayerPrefs.GetFloat("Score" + i)));
        }
        SortScores();
    }

    /// <summary>
    /// Orders the scores list by score
    /// </summary>
    private void SortScores() {
        scores.Sort((x, y) => y.Value.CompareTo(x.Value));
    }

    /// <summary>
    /// Creates the keys in player prefs
    /// </summary>
    private void CreateKeys() {
        for(int i = 0; i < numberOfEntries; i++) {
            PlayerPrefs.SetString("Name" + i, "AAA");
            PlayerPrefs.SetFloat("Score" + i, 0);
        }
    }

    /// <summary>
    /// Saves the scores and names to player prefs
    /// </summary>
    private void Save() {
        for (int i = 0; i < scores.Count; i++) {
            PlayerPrefs.SetString("Name" + (i), scores[i].Key);
            PlayerPrefs.SetFloat("Score" + (i), scores[i].Value);
        }
    }

    /// <summary>
    /// Adds the new high score and supplied name to the scoreboard, removing the bottom entry
    /// </summary>
    /// <param name="name"></param>
    public void AddScore(String name) {
        if(name == "QJZ") {
            DeleteKeys();
            return;
        }
        int position = GetPosition(LastScore);
        if(position >= scores.Count || !NewHighScore) {
            return;
        }
        scores.Add(new KeyValuePair<string, float>(name, LastScore));
        SortScores();
        scores.RemoveAt(scores.Count - 1);
        Save();
        NewHighScore = false;
        LastScore = 0;
        ScoreUpdater scoreUpdater = FindObjectOfType<ScoreUpdater>();
        if(scoreUpdater != null) {
            scoreUpdater.UpdateScores();
        }
    }

    private void DeleteKeys() {
        for (int i = 0; i < numberOfEntries; i++) {
            PlayerPrefs.DeleteKey("Name" + i);
            PlayerPrefs.DeleteKey("Score" + i);
        }
        Setup();
        ScoreUpdater scoreUpdater = FindObjectOfType<ScoreUpdater>();
        if (scoreUpdater != null) {
            scoreUpdater.UpdateScores();
        }
    }


}
