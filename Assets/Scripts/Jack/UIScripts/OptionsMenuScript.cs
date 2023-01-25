using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OptionsMenuScript : MonoBehaviour {
	//Variables.
	[SerializeField]
	private AudioMixer mainMixer = null;

	[SerializeField]
	private Slider volumeSlider = null;

	private void Start()
	{
		float volume = 0.0f;
		if (!PlayerPrefs.HasKey("volume"))
		{
			PlayerPrefs.SetFloat("volume", 0.0f);
		}
		else
		{
			volume = PlayerPrefs.GetFloat("volume");
		}
		UpdateVolume(volume);
	}

	public void UpdateVolume(float value) {
		//Update the volume mixer volume.
		mainMixer.SetFloat("volume", value);
		PlayerPrefs.SetFloat("volume", value);
		volumeSlider.value = value;
	}

	public void LoadHighScores() {
		SceneManager.LoadScene(3);
    }
}
