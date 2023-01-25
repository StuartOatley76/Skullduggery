using UnityEngine;

/// <summary>
/// Subclass of Roompiece to handle a piece with a door in
/// </summary>
public class DoorPiece : RoomPiece
{
    /// <summary>
    /// The door information for this piece's door 
    /// </summary>
    public Room.Door ThisDoor { get; private set; }

    /// <summary>
    /// Override to handle setting up the teleporter and materials
    /// </summary>
    public override GameObject PieceInstance {
        get { return base.PieceInstance; }
        set {
            base.PieceInstance = value;
            base.PieceInstance.GetComponent<Teleporter>().ThisDoor = ThisDoor;
            SetDoorMaterial();
        }
    }

    /// <summary>
    /// Passes the information to DoorFunctionality to apply the correct materials
    /// </summary>
    private void SetDoorMaterial() {
        if(PieceInstance.GetComponent<DoorFunctionality>() != null) {
            PieceInstance.GetComponent<DoorFunctionality>().SetDoorMaterials(PieceStyle, ThisDoor);
        }
    }

    /// <summary>
    /// Overridden constructor
    /// </summary>
    /// <param name="type">Type of piece</param>
    /// <param name="rotation">Rotation of piece</param>
    /// <param name="room">Room Gameobject</param>
    /// <param name="positionInRoom">Position in the room</param>
    /// <param name="style">Room style</param>
    /// <param name="door">The door</param>
    public DoorPiece(RoomPieceType type, Rotation rotation, GameObject room, Vector3 positionInRoom, RoomStyle.RoomStyleType style, Room.Door door) : base(type, rotation, room, positionInRoom, style) {
        ThisDoor = door;

    }
}
