using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Override to set up a rectangular room
/// </summary>
public class RectangularRoomLayout : RoomLayout {
    public RectangularRoomLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO, int xSize, int ySize, RoomStyle.RoomStyleType style, CutCornerStyle cornerStyle = (CutCornerStyle)(-1), int numberOfCornersToCut = -1,
        int cornerSizeX = -1, int cornerSizeY = -1) 
        : base(doors, type, mapGO, xSize, ySize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY) {
    }

    /// <summary>
    /// Creates a room piece of the right type and correct rotation for each position in the room 
    /// </summary>
    protected override void SetupLayout() {
        
        for (int x = 0; x < XLength; x++) {
            for (int y = 0; y < YLength; y++) {
                if (x == 0) {
                    if (y == 0) { // Top left corner
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                        continue;
                    }
                    if (y == YLength - 1) { // Top right corner
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                        continue;
                    }
                    // Top Wall
                    layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (x == XLength - 1) {
                    if (y == 0) { // Bottom left corner
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                        continue;
                    }
                    if (y == YLength - 1) { // Bottom Right corner
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                        continue;
                    }
                    // Bottom Wall
                    layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (y == 0) { // Left wall
                    layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (y == YLength - 1) { // right wall
                    layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                    continue;
                }
                // floor piece
                layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);

            }
        }
    }

    /// <summary>
    /// Replaces appropriate piece with the correct type of door piece based on position in the room
    /// </summary>
    /// <param name="doors"></param>
    protected override void PlaceDoors(List<Room.Door> doors) {

        foreach(Room.Door door in doors) {
            switch (door.Direction) {
                case Room.Direction.Up:
                    if (layout[XLength / 2 - 1, 0] == null && layout[XLength / 2 + 1, 0] == null) {
                        //CorridorEndDoor
                    }
                    if(layout[XLength / 2 - 1, 0] == null) {
                        layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                        continue;
                    }
                    if (layout[XLength / 2 + 1, 0] == null) {
                        layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                        continue;
                    }
                    layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                    continue;

                case Room.Direction.Right:
                    if(layout[XLength - 1, YLength / 2 - 1] == null && layout[XLength - 1, YLength / 2 + 1] == null) {
                        //corridorEndPiece
                    }
                    if (layout[XLength - 1, YLength / 2 - 1] == null) {
                        layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                        continue;
                    }
                    if (layout[XLength - 1, YLength / 2 - 1] == null) {
                        layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                        continue;
                    }
                    layout[XLength-1, YLength/2] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Down, room, new Vector3(XLength -1, 0, YLength / 2), style, door);
                    continue;

                case Room.Direction.Down:
                    if(layout[XLength / 2 - 1, YLength - 1] == null && layout[XLength / 2 + 1, YLength - 1] == null) {
                        //Corridor end
                    }
                    if (layout[XLength / 2 - 1, YLength - 1] == null) {
                        layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                        continue;
                    }
                    if (layout[XLength / 2 + 1, YLength - 1] == null) {
                        layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                        continue;
                    }
                    layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                    continue;

                case Room.Direction.Left:
                    if(layout[0, YLength / 2 - 1] == null && layout[0, YLength / 2 + 1] == null) {
                        //Corridor end
                    }
                    if (layout[0, YLength / 2 - 1] == null) {
                        layout[0, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength / 2), style, door);
                        continue;
                    }
                    if (layout[0, YLength / 2 + 1] == null) {
                        layout[0, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength / 2), style, door);
                        continue;
                    }
                    layout[0, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength / 2), style, door);
                    continue;
                default:
                    continue;
            }
        }
    }
}
