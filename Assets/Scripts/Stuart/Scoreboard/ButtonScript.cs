using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pretend button used in high score entry
/// </summary>
public class ButtonScript : MonoBehaviour
{
    /// <summary>
    /// The unselected icon
    /// </summary>
    [SerializeField] private GameObject unselected;

    /// <summary>
    /// The selected icon
    /// </summary>
    [SerializeField] private GameObject selected;

    /// <summary>
    /// Sets to unselected on awake
    /// </summary>
    private void OnEnable() {
        unselected.SetActive(true);
        selected.SetActive(false);
    }

    /// <summary>
    /// sets selected state
    /// </summary>
    /// <param name="isSelected"></param>
    public void SetSelected(bool isSelected) {
        unselected.SetActive(!isSelected);
        selected.SetActive(isSelected);
    }
}
