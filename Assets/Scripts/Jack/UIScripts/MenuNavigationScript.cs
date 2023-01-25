using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuNavigationScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerialiseFields).
	[SerializeField]
	[Range(0.1f, 5.0f)]
	private float changeButtonCooldownTime = 1.0f;

	[SerializeField]
	[Range(0.1f, 10.0f)]
	private float sliderSpeedMultiplier = 5.0f;

	[SerializeField]
	protected List<RectTransform> menuItems = new List<RectTransform>();

	[SerializeField]
	protected Sprite menuOptionImage = null;

	[SerializeField]
	protected Sprite menuOptionSelected = null;
	#endregion

	#region Private Variables.
	//Changing Button Variables.
	private RectTransform currentItem = null;
	protected virtual RectTransform NewItem { get; set; } = null;
	private Vector2 inputDir = Vector2.zero;
	private Vector2 currentButtonOriginalDimensions = Vector2.zero;
	private Vector2 newButtonOriginalDimesnison = Vector2.zero;
	private bool changingButton = false;
	private bool usingButton = false;
	private bool firstFrame = true;
	private float timer = 0.0f;

	//Enum for menu item type.
	enum MenuItemType {
		Slider,
		Button
	}
	#endregion

	public static event EventHandler ButtonSelect;

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		timer = 0.0f;
		if (menuItems.Count > 0) {
			currentItem = menuItems[0];
			currentButtonOriginalDimensions = currentItem.sizeDelta;
		}
	}

	// Update is called once per frame
	protected virtual void Update() {
		if (Camera.main != null) {
			if (firstFrame) {
				UpdateMenuSelection(Vector2.zero, 1);
			}
			if (!changingButton) {
				//Get the direction of the left stick on the gamepad.
				inputDir = GetPlayerLeftStickInput();

				//Player interacting with menu items code.
				if (currentItem && Input.GetAxisRaw("Gamepad_A") > 0.0f && !usingButton) {
					//Check if the current menu item is a button.
					Button menuButton = currentItem.gameObject.GetComponent<Button>();
					if (menuButton) {
						//TODO::Add some sort of reaction animation here.
						//If it is a button activate it.
						//Start cooldown.
						usingButton = true;
						StartCoroutine(UseButtonCooldown());
						menuButton.onClick.Invoke();
					}
				}
			}

			//Player switching between menu items code is here.
			if (inputDir.magnitude > 0.6f && !changingButton) {
				//Check if the current item is a slider.
				//If it is a slider then make the min direction similarity to change items higher so that slider can be moved.
				float minSimilarity = 0.0f;
				bool horizontal = true;
				Slider menuSlider = currentItem.gameObject.GetComponent<Slider>();
				if (menuSlider) {
					minSimilarity = 0.6f;
					horizontal = menuSlider.direction == Slider.Direction.LeftToRight || menuSlider.direction == Slider.Direction.RightToLeft;
				}

				NewItem = UpdateMenuSelection(inputDir, minSimilarity);
				if (NewItem) {
					//If the new item is valid, start cooldown for selecting a new button.
					changingButton = true;
					StartCoroutine(ChangeButtonCooldown());
				} else {
					if (menuSlider) {
						float sliderIncrement = (menuSlider.maxValue - menuSlider.minValue) * 0.05f;
						//If it's a horizontal slider check left/right input.
						if (horizontal) {
							float direction = 0.0f;
							if (inputDir.x < 0) {
								direction = -1.0f;
							} else {
								direction = 1.0f;
							}

							menuSlider.value += sliderIncrement * direction * Time.unscaledDeltaTime * sliderSpeedMultiplier;
						} else {
							float direction = 0.0f;
							if (inputDir.y < 0) {
								direction = -1.0f;
							} else {
								direction = 1.0f;
							}

							menuSlider.value += sliderIncrement * direction * Time.unscaledDeltaTime * sliderSpeedMultiplier;
						}
					}
				}


			} else {
				if (NewItem && changingButton) {
					inputDir = Vector2.zero;
					newButtonOriginalDimesnison = NewItem.sizeDelta;
					AnimateButtonChange();
				}
			}
		}
	}

	private void OnDisable() {
		//StopAllCoroutines();
		//changingButton = false;
		//usingButton = false;
	}

	protected virtual void OnEnable()
	{
		usingButton = true;
		changingButton = true;
		StartCoroutine(UseButtonCooldown());
		StartCoroutine(ChangeButtonCooldown());
	}

	protected RectTransform UpdateMenuSelection(Vector2 a_inputDir, float minDirectionSimilarity) {
		if (menuItems.Count > 1) {
			List<RectTransform> tempList = new List<RectTransform>();
			for (int i = 0; i < menuItems.Count; i++) {
				tempList.Add(menuItems[i]);
			}
			tempList.Remove(currentItem);

			float lowestDistance = float.MaxValue;
			float mostSimilar = 0.0f - float.MaxValue;
			RectTransform closestItem = currentItem;
			Button button = closestItem.GetComponent<Button>();
			if (button && menuOptionImage) {
                //Set the current item's image to unselected.
                SetUnselectedImage(button);
            }
            foreach (RectTransform item in tempList) {
				button = item.GetComponent<Button>();
				if (button && menuOptionImage) {
					//Set the item's image to unselected.
					SetUnselectedImage(button);
				}
				float distance = Vector2.Distance(item.anchoredPosition, currentItem.anchoredPosition + (a_inputDir));
				float direction = Vector2.Dot(a_inputDir, (item.anchoredPosition - currentItem.anchoredPosition).normalized);
				if (distance < lowestDistance && direction >= mostSimilar && direction > minDirectionSimilarity) {
					lowestDistance = distance;
					mostSimilar = direction;
					closestItem = item;
				}
			}

			button = null;
			button = closestItem.GetComponent<Button>();
			if (button && menuOptionSelected) {
				//Set the item's image to selected.
				button.image.sprite = menuOptionSelected;
			}

			if (closestItem != currentItem) {
				return closestItem;
			}
		}
		return null;
	}

    protected virtual void SetUnselectedImage(Button button) {
		button.image.sprite = menuOptionImage;
    }

    private IEnumerator ChangeButtonCooldown() {
		ButtonSelect?.Invoke(this, EventArgs.Empty);
		yield return new WaitForSecondsRealtime(changeButtonCooldownTime);
		changingButton = false;
	}

	private IEnumerator UseButtonCooldown() {
		yield return new WaitForSecondsRealtime(changeButtonCooldownTime);
		usingButton = false;
	}

	private void AnimateButtonChange() {
		//THIS CODE ANIMATES THE BUTTON CHANGE, CAN BE CHANGED I ONLY PUT THIS IS AS A STOP GAP TILL BETTER ANIMATION IS CREATED.
		//Reset changing button variables.
		currentItem.sizeDelta = currentButtonOriginalDimensions;
		currentItem = NewItem;
		currentButtonOriginalDimensions = newButtonOriginalDimesnison;
		NewItem = null;
		newButtonOriginalDimesnison = Vector2.zero;

	}
	#endregion

	#region Player Input Functions.

	private Vector2 GetPlayerLeftStickInput() {
		Vector2 input = new Vector2(Input.GetAxisRaw("Gamepad_Horizontal"), Input.GetAxisRaw("Gamepad_Vertical"));
		return input;
	}
	#endregion
}
