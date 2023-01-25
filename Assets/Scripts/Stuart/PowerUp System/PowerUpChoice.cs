using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// I have no idea what this is. Probably an early version of the powerup menu.
/// Don't want to delete it in case something breaks though 
/// </summary>
public class PowerUpChoice : MonoBehaviour
{
    [SerializeField] private GameObject PowerUpMenu;
    public void ActivateMenu() {
        PowerUpMenu.SetActive(true);
    }
}
