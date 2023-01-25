using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Class to handle adding an explodepieces to each room piece then triggering it
/// </summary>
public class ExplodeRoom : MonoBehaviour
{
    /// <summary>
    /// List of all the explodePieces in the room
    /// </summary>
    private List<ExplodePieces> roomPieces = new List<ExplodePieces>();

    //Events for timing sound effects
    public static EventHandler RoomExplosion;
    public static EventHandler RoomAssemblyStarted;
    public static EventHandler RoomAssemblyForceFinished;

    /// <summary>
    /// Whether the room has been assembled
    /// </summary>
    public bool HasBeenAssembled { get; private set; } = false;
    public bool IsReassembled { 
        get {
            foreach(ExplodePieces piece in roomPieces) {
                if(piece.IsAssembled == false) {
                    return false;
                }
            }
            return true;
        } 
    }

    /// <summary>
    /// Starts coroutine and triggers event
    /// </summary>
    /// <param name="isPremadeRoom"></param>
    public void ReassembleRoom(bool isPremadeRoom) {
        StartCoroutine(Reassemble(isPremadeRoom));
        RoomAssemblyStarted?.Invoke(this, EventArgs.Empty);
        HasBeenAssembled = true;
    }

    /// <summary>
    /// Creates ExplodePieces on each piece of the room, waits while everything is prepared and then triggers them
    /// </summary>
    /// <param name="isPremadeRoom"></param>
    /// <returns></returns>
    private IEnumerator Reassemble(bool isPremadeRoom) {
        RoomExplosion?.Invoke(this, EventArgs.Empty);
        Transform roomPieceHolder = (isPremadeRoom) ? transform.GetChild(0) : transform;
        List<GameObject> pieces = new List<GameObject>();
        foreach(Transform child in roomPieceHolder) {
            pieces.Add(child.gameObject);
        }
        foreach (GameObject piece in pieces) {
            if (piece.GetComponent<ExplodePieces>() == null) {
                roomPieces.Add(piece.AddComponent<ExplodePieces>());
            } else {
                roomPieces.Add(piece.GetComponent<ExplodePieces>());
            }
        }
        yield return null;
        foreach(ExplodePieces explode in roomPieces) {
            explode.Explode();
        }
    }
}
