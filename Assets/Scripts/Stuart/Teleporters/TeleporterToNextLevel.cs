using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Subclass to set the next level
/// </summary>
public class TeleporterToNextLevel : TeleporterToGame
{

    /// <summary>
    /// Sets next level on enemy spawner
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    //protected override IEnumerator LeaveRoom(GameObject player) {
    //    EnemySpawner.NewLevel();
    //    TrapSpawner.NewLevel();
    //    return base.LeaveRoom(player);
    //}


    /// <summary>
    /// Leaves the level and goes to the high score board
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    protected override IEnumerator LeaveRoom(GameObject player) {
        Scoreboard.instance.CheckScore(this, EventArgs.Empty);
        FindObjectOfType<CameraFade>().ToggleFade();
        yield return null;
        SceneManager.LoadScene("TrophyScene");
    }
}
