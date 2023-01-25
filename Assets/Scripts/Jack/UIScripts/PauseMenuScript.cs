using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private GameObject menuItems = null;

	[SerializeField]
	private GameObject optionsMenu = null;
	#endregion

	#region Private Variables.

	private bool coolingDown = false;
	private bool isMenuOpen = false;
	private bool shoudResumeTime = false;
    #endregion

    #region EventHandlers

    public EventHandler pauseMenuActivated;
	public EventHandler pauseMenuDeactivated;

    #endregion
    #region Private Functions.
    // Start is called before the first frame update
    void Start() {

	}

	// Update is called once per frame
	void Update() {

		if (Input.GetButtonDown("Gamepad_Pause") && !coolingDown) {
			//Pause/Unpause Game.
			SwitchPauseMode();
			coolingDown = true;
			StartCoroutine(SwitchCooldown());
		}
	}

    private void SwitchPauseMode() {
		if (isMenuOpen) {
			Unpause();
		} else {
			Pause();
		}
	}

	private IEnumerator SwitchCooldown() {
		yield return new WaitForSecondsRealtime(1.0f);
		coolingDown = false;
	}
	#endregion

	#region Public Access Functions (Getters and Setters).

	public void Unpause() {
		if (shoudResumeTime) {
			Time.timeScale = 1.0f;
		}
		optionsMenu.SetActive(false);
		menuItems.SetActive(false);
		pauseMenuDeactivated?.Invoke(this, EventArgs.Empty);
		isMenuOpen = false;
	}

	public void Pause() {
		isMenuOpen = true;
		pauseMenuActivated?.Invoke(this, EventArgs.Empty);
		if (Time.timeScale == 0f) {
			shoudResumeTime = false;
		} else {
			shoudResumeTime = true;
			Time.timeScale = 0.0f;
		}
		optionsMenu.SetActive(false);
		menuItems.SetActive(true);
	}

	public void ReturnToMainMenu() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(0);
	}
	#endregion
}
