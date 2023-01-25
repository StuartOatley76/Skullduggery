using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle spawning the player at a fixed point in the tutorial starting room and
/// ensure doors in the starting tutorial room are open
/// </summary>
[RequireComponent(typeof(PlayerSpawner))]
public class Tutorial : MonoBehaviour
{
    /// <summary>
    /// The point to spawn the player at
    /// </summary>
    [SerializeField] Transform startingPoint;
    
    void Start()
    {
        //Opens doors
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach(Animator animator in animators) {
            animator.SetBool("Open", true);
        }
        ///Spawns player
        GetComponent<PlayerSpawner>().SpawnPlayer(startingPoint.position);
    }

}
