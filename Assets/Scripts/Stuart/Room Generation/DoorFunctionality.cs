using UnityEngine;

/// <summary>
/// Class to handle material change for doors and door signs based on the type of room
/// </summary>
public class DoorFunctionality : MonoBehaviour
{
    // parts of the door that need material change
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject doorSign;

    /// <summary>
    /// Sets the appropriate materials
    /// </summary>
    /// <param name="pieceStyle"></param>
    /// <param name="door"></param>
    public void SetDoorMaterials(RoomStyle.RoomStyleType pieceStyle, Room.Door door) {
        Room.RoomType type = door.GetConnectedRoomType();
        Material doorMat = leftDoor.GetComponent<Renderer>().material;
        Material signMat = doorSign.GetComponent<Renderer>().material;

        switch (type) {
            case Room.RoomType.MainPath:
                doorMat = AllRoomStyles.GetStyle(pieceStyle).MainPathDoorMat;
                signMat = AllRoomStyles.GetStyle(pieceStyle).MainPathSignMat;
                break;
            case Room.RoomType.Boss:
                doorMat = AllRoomStyles.GetStyle(pieceStyle).BossRoomDoorMat;
                signMat = AllRoomStyles.GetStyle(pieceStyle).BossRoomSignMat;
                break;
            case Room.RoomType.Starting:
                doorMat = AllRoomStyles.GetStyle(pieceStyle).MainPathDoorMat;
                signMat = AllRoomStyles.GetStyle(pieceStyle).MainPathSignMat;
                break;
            case Room.RoomType.Challange:
                doorMat = AllRoomStyles.GetStyle(pieceStyle).ChallangeRoomDoorMat;
                signMat = AllRoomStyles.GetStyle(pieceStyle).ChallangeRoomSignMat;
                break;
            default:
                break;
        }
        if (doorMat != null) {
            leftDoor.GetComponent<Renderer>().material = doorMat;
            rightDoor.GetComponent<Renderer>().material = doorMat;
            doorSign.GetComponent<Renderer>().material = signMat;
        }
    }
}

