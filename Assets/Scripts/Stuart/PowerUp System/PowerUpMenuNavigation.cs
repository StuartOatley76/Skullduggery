using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Subclass of Jack's Menu Navigation Script to enable power up selection
/// Overrides some parts where different functionality is needed
/// </summary>
public class PowerUpMenuNavigation : MenuNavigationScript
{
    /// <summary>
    /// Event for when a button is pressed
    /// </summary>
    public EventHandler<ButtonBressedEventArgs> OnButtonPressed;

    /// <summary>
    /// List of the selected and unselected sprites for the options available
    /// </summary>
    public List<SelectedAndUnselectedSprites> Sprites { private get; set; }

    /// <summary>
    /// Number of power ups in the menu
    /// </summary>
    public int NumberOfPowerUps { get { return menuItems.Count; } }

    /// <summary>
    /// Override of accessor that allows the options to change when set
    /// </summary>
    protected override RectTransform NewItem {
        get { return base.NewItem; }
        set {
            ChangeMenuOptions(value);
            base.NewItem = value;
        } 
    }

    /// <summary>
    /// Changes the menu options
    /// </summary>
    /// <param name="value"></param>
    private void ChangeMenuOptions(RectTransform value) {
        int index = menuItems.FindIndex(r => r == value);
        if(index == -1) {
            return;
        }
        menuOptionImage = Sprites[index].Unselected;
        menuOptionSelected = Sprites[index].Selected;
    }

    /// <summary>
    /// Sets up the menu
    /// </summary>
    protected override void OnEnable() {
        if (Sprites != null && Sprites.Count != 0) {
            menuOptionImage = Sprites[0].Unselected;
            menuOptionSelected = Sprites[0].Selected;
            for (int i = 0; i < menuItems.Count && i < Sprites.Count; i++) {
                Image image = menuItems[i].GetComponent<Image>();
                if (image != null) {
                    image.sprite = Sprites[i].Unselected;
                }
                Button button = menuItems[i].GetComponent<Button>();
                if(button != null) {
                    SpriteState ss = new SpriteState();
                    ss.highlightedSprite = Sprites[i].Selected;
                    ss.pressedSprite = Sprites[i].Selected;
                    ss.selectedSprite = Sprites[i].Selected;
                    button.spriteState = ss;
                    int x = i;
                    button.onClick.AddListener(() => ButtonClicked(x));
                }
            }
        }
        
        base.OnEnable();
    }

    /// <summary>
    /// Allows B button to exit without selection
    /// </summary>
    protected override void Update() {
        if (Input.GetButtonDown("Gamepad_B")) {
            ButtonClicked(4);
        }
        base.Update();
    }

    /// <summary>
    /// Triggers event when button clicked
    /// </summary>
    /// <param name="i"></param>
    private void ButtonClicked(int i) {
        OnButtonPressed?.Invoke(this, new ButtonBressedEventArgs(i));
    }

    /// <summary>
    /// Sets the button to unselected
    /// </summary>
    /// <param name="button"></param>
    protected override void SetUnselectedImage(Button button) {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        int index = menuItems.FindIndex(r => r == rectTransform);
        button.image.sprite = Sprites[index].Unselected;
    }
}

/// <summary>
/// Class to hold the selected and unselected sprites for a button
/// </summary>
public class SelectedAndUnselectedSprites {
    public Sprite Unselected { get; private set; }
    public Sprite Selected { get; private set; }

    public SelectedAndUnselectedSprites(Sprite unselected, Sprite selected) {
        Unselected = unselected;
        Selected = selected;
    }
}

/// <summary>
/// Event ears for when a button is pressed
/// </summary>
public class ButtonBressedEventArgs : EventArgs {
    public int ButtonID { get; private set; }

    public ButtonBressedEventArgs(int id) {
        ButtonID = id;
    }
}




