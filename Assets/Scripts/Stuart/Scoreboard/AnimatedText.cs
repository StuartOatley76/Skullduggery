using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Class to handle moving the letters on the high score board entry
/// </summary>
public class AnimatedText : MonoBehaviour
{
    /// <summary>
    /// The text object attached to this
    /// </summary>
    private Text text;
    public string Text {
        get {
            return text.text;
        }
        set {
            text.text = value;
        }
    }

    /// <summary>
    /// The starting position when enabled.
    /// used to reset ready for next entry
    /// </summary>
    private Vector3 startingPosition;

    /// <summary>
    /// The starting rotation. As above
    /// </summary>
    private Quaternion startingRotation;

    /// <summary>
    /// The position at the start of animation
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// The rotation at the start of animation
    /// </summary>
    private Quaternion originalRotation;

    /// <summary>
    /// The target position at the end of the animation
    /// </summary>
    private Vector3 destinationPosition;

    /// <summary>
    /// The target rotation at the end of the animation
    /// </summary>
    private Quaternion destinationRotation;

    /// <summary>
    /// Whether the text is being animated
    /// </summary>
    public bool IsAnimating { get; private set; } = false;

    /// <summary>
    /// How long it takes to complete the animation
    /// </summary>
    private float timeToMove = 0.3f;

    /// <summary>
    /// Gameobject holding the unselected image. Child object as 
    /// text is an image type and a gameobject can't have 2 images on it
    /// </summary>
    [SerializeField] private GameObject unselected;

    /// <summary>
    /// Gameobject holding the selected image. As above
    /// </summary>
    [SerializeField] private GameObject selected;

    
    /// <summary>
    /// Finds text componant and sets starting values
    /// </summary>
    void Awake()
    {
        text = GetComponent<Text>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    /// <summary>
    /// unselects when disabled
    /// </summary>
    private void OnDisable() {
        SetSelected(false);
    }

    /// <summary>
    /// Sets which image to have active
    /// </summary>
    /// <param name="isSelected"></param>
    public void SetSelected(bool isSelected) {
        unselected.SetActive(!isSelected);
        selected.SetActive(isSelected);
    }

    /// <summary>
    /// Moves the text directly without animation
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    public void MoveDirectly(Vector3 targetPosition, Quaternion targetRotation) {
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    /// <summary>
    /// Sets up ready to animate and starts thhe coroutine that animates
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    public void AnimateTo(Vector3 targetPosition, Quaternion targetRotation) {
        if (IsAnimating) {
            return;
        }
        IsAnimating = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        destinationPosition = targetPosition;
        destinationRotation = targetRotation;
        StartCoroutine(Animate());
    }

    /// <summary>
    /// Performs the animation using slerps
    /// </summary>
    /// <returns></returns>
    private IEnumerator Animate() {
        
        float startingTime = Time.time;
        while(startingTime + timeToMove > Time.time) {
            transform.position = Vector3.Slerp(originalPosition, destinationPosition, (Time.time - startingTime) / timeToMove);
            transform.rotation = Quaternion.Slerp(originalRotation, destinationRotation, (Time.time - startingTime) / timeToMove);
            yield return null;
        }
        transform.position = destinationPosition;
        transform.rotation = destinationRotation;
        IsAnimating = false;
    }

    /// <summary>
    /// Sets back to original position and rotation
    /// </summary>
    public void ResetPosition() {
        transform.position = startingPosition;
        transform.rotation = startingRotation;
    }
}
