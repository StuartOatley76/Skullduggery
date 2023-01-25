using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to spawn traps
/// </summary>
public static class TrapSpawner
{
    /// <summary>
    /// Maximum number of traps per room
    /// </summary>
    private static int maxNumberOfTraps = 3;

    /// <summary>
    /// Spawns traps with similar rest
    /// </summary>
    /// <param name="room"></param>
    /// <param name="currentRoom"></param>
    /// <param name="numberOfroomsOnMainPath"></param>

    //Static constructor. Connects to player dying event to reset
    static TrapSpawner() {
        CharacterController.PlayerKilled += Reset;
    }

    /// <summary>
    /// resets the maximum number of traps
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void Reset(object sender, System.EventArgs e) {
        maxNumberOfTraps = 3;
    }

    /// <summary>
    /// Increases the maximum number of traps
    /// </summary>
    public static void NewLevel() {
        maxNumberOfTraps += 2;
    }

    /// <summary>
    /// Spawns traps with restrictions based on how far through the level the player is
    /// </summary>
    /// <param name="room"></param>
    /// <param name="currentRoom"></param>
    /// <param name="numberOfroomsOnMainPath"></param>
    public static void SpawnTraps(GameObject room, int currentRoom, int numberOfroomsOnMainPath) {
        if(room.GetComponent<RoomInformation>() == null) {
            return;
        }
        int numberOfroomsPerTrapIncrease = numberOfroomsOnMainPath / (maxNumberOfTraps + 1);
        int maxTrapsForThisRoom;
        if (numberOfroomsPerTrapIncrease > 0) {
            maxTrapsForThisRoom = currentRoom / numberOfroomsPerTrapIncrease;
        } else {
            maxTrapsForThisRoom = 0;
        }
        if(room.GetComponent<RoomInformation>().Type == Room.RoomType.Challange) {
            maxTrapsForThisRoom = maxNumberOfTraps;
        }
        if(maxTrapsForThisRoom == 0) {
            return;
        }
        int numberOfTraps = Random.Range(0, maxTrapsForThisRoom + 1);
        if(numberOfTraps == 0) {
            return;
        }
        numberOfTraps = (numberOfTraps > maxNumberOfTraps) ? maxNumberOfTraps : numberOfTraps;
        List<GameObject> floorpieces = room.GetComponent<RoomInformation>().FloorPieces;
        List<GameObject> traps = AllRoomStyles.GetStyle(room.GetComponent<RoomInformation>().Style.Type).GetTrapPrefabs();

        if(traps.Count == 0) {
            return;
        }
        for(int i = 0; i < numberOfTraps; i++) {
            GameObject floorPiece = floorpieces[Random.Range(0, floorpieces.Count)];
            floorpieces.Remove(floorPiece);
            GameObject trapPrefab = traps[Random.Range(0, traps.Count)];
            GameObject trap = GameObject.Instantiate(trapPrefab, floorPiece.transform.position, floorPiece.transform.rotation, room.transform);
            AllRoomStyles.GetStyle(room.GetComponent<RoomInformation>().Style.Type).SetMaterial(trap, room.GetComponent<RoomInformation>().Type);
            GameObject.Destroy(floorPiece);
        }
    }
}
