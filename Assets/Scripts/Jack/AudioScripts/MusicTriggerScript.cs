using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicTriggerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private AudioFile musicClip = null;

	[SerializeField]
	[Range(0.0f, 10.0f)]
	private float transitionTime = 1.0f;

	[SerializeField]
	private bool loopMusic = true;

	[SerializeField]
	private bool playBaseMusicOnExit = true;
	#endregion

	#region Private Variable Declarations.

	private bool canPlayAgain = true;
	private static string currentClipName = string.Empty;
	#endregion

	#region Private Functions.

	private void Start() {
		currentClipName = string.Empty;
	}

	private void OnTriggerEnter(Collider collision) {
		if (collision.gameObject.tag == "Player" && currentClipName != musicClip.audioClip.name) {
			StopCoroutine(MusicTriggerCooldown());
			canPlayAgain = true;
		}

		if (collision.gameObject.tag == "Player" && canPlayAgain && musicClip != null) {
			currentClipName = musicClip.audioClip.name;
			AudioManagerScript.PlayMusicClip(musicClip, transitionTime, loopMusic);
			canPlayAgain = false;

			if (!loopMusic) {
				StartCoroutine(MusicTriggerCooldown());
			}
		}
	}

	private void OnTriggerExit(Collider collision) {
		if (playBaseMusicOnExit) {
			if (collision.gameObject.tag == "Player") {
				StopCoroutine(MusicTriggerCooldown());
				canPlayAgain = true;
			}

			if (collision.gameObject.tag == "Player" && canPlayAgain) {
				AudioManagerScript.PlayBaseMusic();
				canPlayAgain = false;

				if (!loopMusic) {
					StartCoroutine(MusicTriggerCooldown());
				}
			}
		}
	}

	private IEnumerator MusicTriggerCooldown() {
		yield return new WaitForSeconds(musicClip.audioClip.length);
		canPlayAgain = true;
	}
	#endregion
}
