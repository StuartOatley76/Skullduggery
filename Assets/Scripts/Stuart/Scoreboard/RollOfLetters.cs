using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle a roll of letters to enable entry of one letter into the name
/// </summary>
public class RollOfLetters : MonoBehaviour
{
    /// <summary>
    /// enum to represent distance in ascii from the current letter
    /// </summary>
    private enum Letters {
        Twobefore = 2,
        OneBefore = 1,
        Current = 0,
        OneAfter = -1,
        TwoAfter = -2
    }

    //The 5 animated texts that make up a roll
    [SerializeField] private AnimatedText offScreenTop;
    [SerializeField] private AnimatedText previousLetter;
    [SerializeField] private AnimatedText currentLetter;
    [SerializeField] private AnimatedText nextLetter;
    [SerializeField] private AnimatedText offScreenBottom;

    /// <summary>
    /// List of the above
    /// </summary>
    List<AnimatedText> animatedTexts = new List<AnimatedText>();

    /// <summary>
    /// Arrow image that shows roll is selected
    /// </summary>
    [SerializeField] private GameObject SelectedArrows;

    /// <summary>
    /// Initial letter selected in the roll
    /// </summary>
    private char startingLetter = 'A';
    
    //Lowest and highest in ascii codes the roll goes
    private const int minAsciiValue = 65;
    private const int maxAsciiValue = 91;

    /// <summary>
    /// The current letter in the roll
    /// </summary>
    public char CurrentCharacter { get; private set; }
    private int characterGap;

    /// <summary>
    /// Initialises the letters in the roll
    /// </summary>
    private void Start() {
        characterGap = maxAsciiValue - minAsciiValue;
        CurrentCharacter = startingLetter;
        offScreenTop.Text = CalcChar(CurrentCharacter, Letters.Twobefore).ToString();
        previousLetter.Text = CalcChar(CurrentCharacter, Letters.OneBefore).ToString();
        currentLetter.Text = CurrentCharacter.ToString();
        nextLetter.Text = CalcChar(CurrentCharacter, Letters.OneAfter).ToString();
        offScreenBottom.Text = CalcChar(CurrentCharacter, Letters.TwoAfter).ToString();
        SetupList();
        foreach(AnimatedText text in animatedTexts) {
            text.ResetPosition();
        }
    }

    /// <summary>
    /// activates or deactivates the selection arrows
    /// </summary>
    /// <param name="isSelected"></param>
    public void SetSelected(bool isSelected) {
        SelectedArrows.SetActive(isSelected);
    }

    /// <summary>
    /// Loops through each animated text telling it where to animate to when the roll is moved up
    /// Changes letter on the new bottom one to the next letter
    /// </summary>
    public void MoveUp() {
        if(animatedTexts.Any(a => a.IsAnimating == true)) {
            return;
        }
        AnimatedText moveToBottom = animatedTexts[0];
        Vector3 bottomPos = animatedTexts[animatedTexts.Count - 1].transform.position;
        Quaternion bottomRot = animatedTexts[animatedTexts.Count - 1].transform.rotation;

        for(int i = animatedTexts.Count - 1; i >= 1; i--) {
            Transform target = animatedTexts[i - 1].transform;
            animatedTexts[i].AnimateTo(target.position, target.rotation);
        }
        animatedTexts.Remove(moveToBottom);
        animatedTexts.Add(moveToBottom);
        CurrentCharacter = animatedTexts[animatedTexts.Count / 2].Text[0];
        moveToBottom.MoveDirectly(bottomPos, bottomRot);
        moveToBottom.Text = CalcChar(CurrentCharacter, Letters.TwoAfter).ToString();
    }

    /// <summary>
    /// Loops through each animated text telling it where to animate to when the roll is moved down
    /// Changes letter on the new top one to the next letter
    /// </summary>
    public void MoveDown() {
        if (animatedTexts.Any(a => a.IsAnimating == true)) {
            return;
        }
        AnimatedText moveToTop = animatedTexts[animatedTexts.Count - 1];
        Vector3 topPos = animatedTexts[0].transform.position;
        Quaternion topRot = animatedTexts[0].transform.rotation;

        for (int i = 0; i < animatedTexts.Count - 1; i++) {
            Transform target = animatedTexts[i + 1].transform;
            animatedTexts[i].AnimateTo(target.position, target.rotation);
        }
        animatedTexts.Remove(moveToTop);
        animatedTexts.Insert(0, moveToTop);
        CurrentCharacter = animatedTexts[animatedTexts.Count / 2].Text[0];
        moveToTop.MoveDirectly(topPos, topRot);
        moveToTop.Text = CalcChar(CurrentCharacter, Letters.Twobefore).ToString();
    }

    /// <summary>
    /// calculates the next ascii code, looping around when needed
    /// </summary>
    /// <param name="currentCharacter"></param>
    /// <param name="neededLetter"></param>
    /// <returns></returns>
    private char CalcChar(char currentCharacter, Letters neededLetter) {
        int ascii = (int)currentCharacter - (int)neededLetter;
        if(ascii < minAsciiValue) {
            ascii += characterGap;
        } else if(ascii > maxAsciiValue) {
            ascii -= characterGap;
        }
        return (char)ascii;
    }

    /// <summary>
    /// Sets up the list of animated texts
    /// </summary>
    private void SetupList() {
        animatedTexts.Clear();
        animatedTexts.Add(offScreenTop);
        animatedTexts.Add(previousLetter);
        animatedTexts.Add(currentLetter);
        animatedTexts.Add(nextLetter);
        animatedTexts.Add(offScreenBottom);
    }
}
