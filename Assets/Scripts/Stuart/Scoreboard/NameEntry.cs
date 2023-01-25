using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Class to handle inputting name for the high score board
/// </summary>
public class NameEntry : MonoBehaviour
{
    //3 rolls of letters used to input name
    [SerializeField] private RollOfLetters firstLetter;
    [SerializeField] private RollOfLetters secondLetter;
    [SerializeField] private RollOfLetters thirdLetter;

    /// <summary>
    /// Reference to the back indicator
    /// </summary>
    [SerializeField] private GameObject BackIndicator;
    /// <summary>
    /// Button to input name. Not a real button as don't work well with controller input
    /// </summary>
    [SerializeField] private ButtonScript button;

    /// <summary>
    /// Time to wait between horizontal changes
    /// </summary>
    private float timeBetweenActions = 0.15f;

    /// <summary>
    /// Amount of axis movement to register an action
    /// </summary>
    private float minAxisAmount = 0.5f;

    /// <summary>
    /// Time last horizontal input recieved
    /// </summary>
    private float timeOfLastInput;

    /// <summary>
    /// List of the roll of letters above
    /// </summary>
    private readonly List<RollOfLetters> rolls = new List<RollOfLetters>();
    private int index;

    /// <summary>
    /// Index of which roll of letters currently selected. value of rolls.Count means button
    /// </summary>
    private int Index { get { return index; } 
        set { 
            index = value;
            IndexChange();
        } 
    }

    /// <summary>
    /// The letters entered
    /// </summary>
    private char[] entry;

    /// <summary>
    /// Adds the rolls to the list and initialises the letters and index
    /// </summary>
    private void Awake() {
        rolls.Add(firstLetter);
        rolls.Add(secondLetter);
        rolls.Add(thirdLetter);
        entry = new char[rolls.Count];
        Index = 0;
    }

    /// <summary>
    /// If there's no new high score, hide the object
    /// Resets ready for new name entry
    /// </summary>
    private void OnEnable() {
        if (!Scoreboard.instance.NewHighScore) {
            gameObject.SetActive(false);
            return;
        }
        BackIndicator.SetActive(false);
        Array.Clear(entry, 0, entry.Length);
        Index = 0;
        StartCoroutine(Setup());
        rolls[0].SetSelected(true);
        rolls[1].SetSelected(false);
        rolls[2].SetSelected(false);
    }

    /// <summary>
    /// pause for one frame before setting selected state
    /// </summary>
    /// <returns></returns>
    private IEnumerator Setup() {
        yield return null;
        rolls[0].SetSelected(true);
        rolls[1].SetSelected(false);
        rolls[2].SetSelected(false);
    }

    /// <summary>
    /// sets selected item when the index changes
    /// </summary>
    private void IndexChange() {
        button.SetSelected(Index == rolls.Count);
        foreach(RollOfLetters roll in rolls) {
            if(rolls.FindIndex(r=> r == roll) == index) {
                roll.SetSelected(true);
                continue;
            }
            roll.SetSelected(false);
        }
    }

    /// <summary>
    /// Checks for and applies input
    /// </summary>
    private void Update() {

        if ((Input.GetAxis("Gamepad_Vertical") > minAxisAmount || Input.GetAxis("Gamepad_Vertical") < -minAxisAmount) && index != rolls.Count) {
            if (Input.GetAxis("Gamepad_Vertical") < 0) {
                rolls[index].MoveDown();
            }
            if (Input.GetAxis("Gamepad_Vertical") > 0) {
                rolls[index].MoveUp();
            }
        }

        if (Input.GetButton("Gamepad_A")) {
            timeOfLastInput = Time.time;
            if (index == rolls.Count) {
                SetName();
                return;
            }
            entry[Index] = rolls[Index].CurrentCharacter;
            return;
        }

        if (timeOfLastInput + timeBetweenActions > Time.time) {
            return;
        }

        if(Input.GetAxis("Gamepad_Horizontal") > minAxisAmount || Input.GetAxis("Gamepad_Horizontal") < -minAxisAmount) {
            timeOfLastInput = Time.time;
            if(Input.GetAxis("Gamepad_Horizontal") < -minAxisAmount && Index > 0) {
                Index--;
            }
            if(Input.GetAxis("Gamepad_Horizontal") > minAxisAmount &&  Index < rolls.Count) {
                Index++;
            }
            return;
        }


    }

    /// <summary>
    /// Gets the name, passes it to the scoreboard, and disables thhe entry system
    /// </summary>
    private void SetName() {
        for(int i = 0; i < entry.Length; i++) {
            entry[i] = rolls[i].CurrentCharacter;
        }
        Scoreboard.instance.AddScore(new string(entry));
        gameObject.SetActive(false);
        BackIndicator.SetActive(true);
    }
}
