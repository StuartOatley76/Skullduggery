using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all possible versions of each power up. Selects the three options for the menu and then
/// passes the selected one to the stats multiplier
/// </summary>
public class PowerUpSelector : MonoBehaviour
{
    /// <summary>
    /// list of all the power ups
    /// </summary>
    [SerializeField] private List<PowerUp> powerUps;
    /// <summary>
    /// Default power up for when none is in use
    /// </summary>
    [SerializeField] private PowerUp defaultPowerUp;

    /// <summary>
    /// The gameobject holding the power up menu
    /// </summary>
    [SerializeField] private GameObject powerUpMenu;

    /// <summary>
    /// The pause menu. Accessed to prevent conflicts
    /// </summary>
    [SerializeField] private PauseMenuScript pauseMenu;

    /// <summary>
    /// The powerup menu navigation class on the menu
    /// </summary>
    [SerializeField] private PowerUpMenuNavigation menuNavigation;

    // All the power ups split into lists by rarity
    private List<PowerUp> commons;
    private List<PowerUp> rares;
    private List<PowerUp> epics;
    private List<PowerUp> legendaries;

    /// <summary>
    /// The selected options available to the player
    /// </summary>
    private List<PowerUp> currentOptions;
   
    /// <summary>
    /// Sets up the options and passes them to the menu
    /// </summary>
    private void OnEnable() {
        FindObjects();
        if (powerUpMenu == null || menuNavigation == null) {
            Debug.LogError("Object not found");
            gameObject.SetActive(false);
            return;
        }
        if (pauseMenu != null) {
            pauseMenu.pauseMenuActivated += Hide;
            pauseMenu.pauseMenuDeactivated += Show;
        }
        ResetLists();
        currentOptions = SelectPowerUps(menuNavigation.NumberOfPowerUps);
        List<SelectedAndUnselectedSprites> sprites = new List<SelectedAndUnselectedSprites>();
        for(int i = 0; i < currentOptions.Count; i++) {
            sprites.Add(new SelectedAndUnselectedSprites(currentOptions[i].UnselectedIcon, currentOptions[i].SelectedIcon));
        }
        menuNavigation.Sprites = sprites;
        menuNavigation.OnButtonPressed += OnSelect;
        powerUpMenu.SetActive(true);
        Time.timeScale = 0;

    }

    /// <summary>
    /// Finds objects that are needed
    /// </summary>
    private void FindObjects() {
        powerUpMenu = Resources.FindObjectsOfTypeAll<PowerUpMenuNavigation>().Where(p => p.GetInstanceID() > -1).First().gameObject;
        pauseMenu = Resources.FindObjectsOfTypeAll<PauseMenuScript>().Where(p => p.GetInstanceID() > -1).First();
        menuNavigation = Resources.FindObjectsOfTypeAll<PowerUpMenuNavigation>().Where(p => p.GetInstanceID() > -1).First();
    }

    /// <summary>
    /// enables the menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Show(object sender, EventArgs e) {
        powerUpMenu.SetActive(true);
    }

    /// <summary>
    /// disables the menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Hide(object sender, EventArgs e) {
        powerUpMenu.SetActive(false);
    }

    /// <summary>
    /// sorts the available powerups into lists based on rarity
    /// </summary>
    private void ResetLists() {
        commons = powerUps.Where(rarity => rarity.Rarity == Rarity.Common).ToList();
        rares = powerUps.Where(rarity => rarity.Rarity == Rarity.Rare).ToList();
        epics = powerUps.Where(rarity => rarity.Rarity == Rarity.Epic).ToList();
        legendaries = powerUps.Where(rarity => rarity.Rarity == Rarity.Legendary).ToList();
    }

    /// <summary>
    /// Delegate called when menu button pressed
    /// Applies the selected powerup if valid, then closes the menu
    /// Invalid one used to exit menu without making selection
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public void OnSelect(object o, ButtonBressedEventArgs e) {
        if (e.ButtonID < currentOptions.Count) {
            PowerUp powerUp = currentOptions[e.ButtonID];
            StatMultiplier.Instance.CurrentPowerUp = Instantiate(powerUp);
        }
        powerUpMenu.SetActive(false);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets up ready to select the powerups ond selects them using SelectWeightedPowerUp
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private List<PowerUp> SelectPowerUps(int length) {
        if(powerUps.Count == length) {
            return powerUps;
        }
        List<PowerUp> selected = new List<PowerUp>();
        if(powerUps.Count < length) {
            selected.AddRange(powerUps);
            while(selected.Count < length) {
                selected.Add(defaultPowerUp);
            }
            return selected;
        }
        for(int i = 0; i < length; i++) {
            selected.Add(SelectWeightedPowerUp());
        }
        foreach(PowerUpObject powerUp in FindObjectsOfType<PowerUpObject>()) {
            Destroy(powerUp.gameObject);
        }
        return selected;

    }

    /// <summary>
    /// Selects the power ups by rarity based on weight 
    /// </summary>
    /// <returns></returns>
    private PowerUp SelectWeightedPowerUp() {
        int choice = UnityEngine.Random.Range(0, 101);
        switch (choice) {
            case var _ when choice <= (int)Rarity.Legendary:
                int legendarySelection = UnityEngine.Random.Range(0, legendaries.Count);
                PowerUp legendarySelected = legendaries[legendarySelection];
                legendaries.Remove(legendarySelected);
                return legendarySelected;
            case var _ when choice <= (int)Rarity.Epic:
                int epicSelection = UnityEngine.Random.Range(0, epics.Count);
                PowerUp epicSelected = epics[epicSelection];
                epics.Remove(epicSelected);
                return epicSelected;
            case var _ when choice <= (int)Rarity.Rare:
                int rareSelection = UnityEngine.Random.Range(0, rares.Count);
                PowerUp rareSelected = rares[rareSelection];
                rares.Remove(rareSelected);
                return rareSelected;
            default:
                int commonSelection = UnityEngine.Random.Range(0, commons.Count);
                PowerUp commonSelected = commons[commonSelection];
                commons.Remove(commonSelected);
                return commonSelected;
        }
    }

    /// <summary>
    /// disconnects events
    /// </summary>
    private void OnDisable() {
        pauseMenu.pauseMenuActivated -= Hide;
        pauseMenu.pauseMenuDeactivated -= Show;

    }
}
