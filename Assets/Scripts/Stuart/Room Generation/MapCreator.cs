using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Class to handle the minimap
/// </summary>
public class MapCreator : MonoBehaviour
{
    /// <summary>
    /// instance
    /// </summary>
    private static MapCreator mapCreator;
    public static MapCreator Map { get { return mapCreator; } }

    /// <summary>
    /// Materials for the different rooms
    /// </summary>

    [SerializeField] private Material currentRoomMat;
    [SerializeField] private Material bossMat;
    [SerializeField] private Material pathMat;
    [SerializeField] private Material challangeMat;

    //objects for the different rooms. Letters are directions of the doors

    [SerializeField] private GameObject D;
    [SerializeField] private GameObject DL;
    [SerializeField] private GameObject L;
    [SerializeField] private GameObject R;
    [SerializeField] private GameObject RD;
    [SerializeField] private GameObject RDL;
    [SerializeField] private GameObject RL;
    [SerializeField] private GameObject U;
    [SerializeField] private GameObject UD;
    [SerializeField] private GameObject UDL;
    [SerializeField] private GameObject UL;
    [SerializeField] private GameObject UR;
    [SerializeField] private GameObject URD;
    [SerializeField] private GameObject URDL;
    [SerializeField] private GameObject URL;

    /// <summary>
    /// Object that holds the map
    /// </summary>
    private GameObject mapHolder;

    /// <summary>
    /// Object holding the Camera for the minimap
    /// </summary>
    [SerializeField] private GameObject cameraGO;
    
    /// <summary>
    /// The room on the map the player is currently in
    /// </summary>
    private GameObject currentRoom;

    /// <summary>
    /// Holds the original material for the room the player is currently in
    /// </summary>
    private Material tempMat;

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake() {
        if(mapCreator != null) {
            Destroy(this);
        }
        mapCreator = this;
    }

    /// <summary>
    /// static function to place a room on the map
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static GameObject PlaceRoom(Room room) {
        return mapCreator.InstantiateRoom(room);
    }


    /// <summary>
    /// Creates the map roompiece based on the information in room
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private GameObject InstantiateRoom(Room room) {
        if(mapHolder == null) {
            mapHolder = GameObject.FindGameObjectWithTag("Minimap");
        }
        GameObject roomGO;
        switch (room.NumberOfDoors) {
            case 1:
                if (room.ContainsDoorDirection(Room.Direction.Up)) {
                    roomGO = Instantiate(D, mapHolder.transform);
                    break;
                }
                if (room.ContainsDoorDirection(Room.Direction.Right)) {
                    roomGO = Instantiate(R, mapHolder.transform);

                    break;
                }
                if (room.ContainsDoorDirection(Room.Direction.Down)) {
                    roomGO = Instantiate(U, mapHolder.transform);
                    break;
                }
                if (room.ContainsDoorDirection(Room.Direction.Left)) {
                    roomGO = Instantiate(L, mapHolder.transform);
                    break;
                }
                roomGO = null;
                break;
            case 2:
                if (room.ContainsDoorDirection(Room.Direction.Up)) {
                    if (room.ContainsDoorDirection(Room.Direction.Right)) {
                        roomGO = Instantiate(RD, mapHolder.transform);
                        break;
                    }
                    if (room.ContainsDoorDirection(Room.Direction.Down)) {
                        roomGO = Instantiate(UD, mapHolder.transform);
                        break;
                    }
                    if (room.ContainsDoorDirection(Room.Direction.Left)) {
                        roomGO = Instantiate(DL, mapHolder.transform);
                        break;
                    }
                    roomGO = null;
                    break;
                }
                if (room.ContainsDoorDirection(Room.Direction.Right)) {
                    if (room.ContainsDoorDirection(Room.Direction.Down)) {
                        roomGO = Instantiate(UR, mapHolder.transform);
                        break;
                    }
                    if (room.ContainsDoorDirection(Room.Direction.Left)) {
                        roomGO = Instantiate(RL, mapHolder.transform);
                        break;
                    }
                    roomGO = null;
                    break;
                }
                roomGO = Instantiate(UL, mapHolder.transform);
                break;

            case 3:
                if (!room.ContainsDoorDirection(Room.Direction.Up)) {
                    roomGO = Instantiate(URL, mapHolder.transform);
                    break;
                }
                if (!room.ContainsDoorDirection(Room.Direction.Right)) {
                    roomGO = Instantiate(UDL, mapHolder.transform);
                    break;
                }
                if (!room.ContainsDoorDirection(Room.Direction.Down)) {
                    roomGO = Instantiate(RDL, mapHolder.transform);
                    break;
                }
                if (!room.ContainsDoorDirection(Room.Direction.Left)) {
                    roomGO = Instantiate(URD, mapHolder.transform);
                    break;
                }
                roomGO = null;
                break;
            case 4:
                roomGO = Instantiate(URDL, mapHolder.transform);
                break;
            default:
                return null;
        }
        if(roomGO == null) {
            return null;
        }
        roomGO.transform.localPosition = new Vector3(room.Position.x, 0, -room.Position.y);
        Material matToUse = pathMat;
        switch (room.roomType) {
            case Room.RoomType.Starting:
                matToUse = pathMat;
                break;
            case Room.RoomType.MainPath:
                matToUse = pathMat;
                break;
            case Room.RoomType.Boss:
                matToUse = bossMat;
                break;
            case Room.RoomType.Challange:
                matToUse = challangeMat;
                break;
            default:
                break;
        }
        Renderer renderer = roomGO.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material = matToUse;
        }
        roomGO.SetActive(false);
        return roomGO;
    }

    /// <summary>
    /// stores material on the room entered and replaces it with current room material
    /// </summary>
    /// <param name="room"></param>
    public static void EnterRoom(GameObject room) {
        if (!room) {
            return;
        }
        if(room.activeSelf == false) {
            room.SetActive(true);
        }
        Renderer renderer = null;
        if (mapCreator && mapCreator.cameraGO) {
            mapCreator.cameraGO.transform.localPosition = new Vector3(room.transform.localPosition.x, mapCreator.cameraGO.transform.localPosition.y, room.transform.localPosition.z);
            mapCreator.currentRoom = room;
            renderer = room.GetComponent<Renderer>();
        }
        if (renderer != null) {
            mapCreator.tempMat = renderer.material;
            renderer.material = mapCreator.currentRoomMat;
        }
        if (!mapCreator.cameraGO.activeSelf) {
            mapCreator.cameraGO.SetActive(true);
        }
    }

    /// <summary>
    /// Replaces original material
    /// </summary>
    public static void ExitRoom() {
        Renderer renderer = null;
        if (mapCreator && mapCreator.currentRoom) {
             renderer = mapCreator.currentRoom.GetComponent<Renderer>();
        }
        if(renderer != null) {
            renderer.material = mapCreator.tempMat;
        }
    }
}
