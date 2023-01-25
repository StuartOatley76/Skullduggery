using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Class to handle main menu functionality
/// </summary>
public class Menu : MonoBehaviour {

    CameraFade cameraFade;
    private void Start() {
        cameraFade = FindObjectOfType<CameraFade>();
        if(cameraFade && cameraFade.IsFaded) {
            cameraFade.ToggleFade();
        }
    }

    /// <summary>
    /// Loads the given scene
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(int scene) {
		
		if(cameraFade != null) {
			StartCoroutine(Fade(cameraFade, scene));
			return;
        }
        EnemySpawner.Level = 1;
		SceneManager.LoadScene(scene);
	}

    private IEnumerator Fade(CameraFade cameraFade, int scene) {
		cameraFade.ToggleFade();
        while (cameraFade.Fading) {
			yield return null;
        }
		SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit() {
#if UNITY_EDITOR
		if (EditorApplication.isPlaying) {
			UnityEditor.EditorApplication.isPlaying = false;
		}
#endif
		Application.Quit();
	}
}
