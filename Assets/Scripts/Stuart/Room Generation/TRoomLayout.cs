using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subclass of rectangular room layout to create a T rool layout
/// </summary>
public class TRoomLayout : RectangularRoomLayout
{
    /// <summary>
    /// The 2 corners that still remain
    /// </summary>
    private CornerPlacement[] existingCorners = new CornerPlacement[2];
    /// <summary>
    /// constructor. Same as rectangular room layout's
    /// </summary>
    /// <param name="doors"></param>
    /// <param name="type"></param>
    /// <param name="mapGO"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    /// <param name="style"></param>
    /// <param name="cornerStyle"></param>
    /// <param name="numberOfCornersToCut"></param>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    public TRoomLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO, int xSize, int ySize, RoomStyle.RoomStyleType style = (RoomStyle.RoomStyleType)(-1), CutCornerStyle cornerStyle = (CutCornerStyle)(-1), 
        int numberOfCornersToCut = -1, int cornerSizeX = -1, int cornerSizeY = -1) : base(doors, type, mapGO, xSize, ySize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY) {

    }

    /// <summary>
    /// Override for Set Types to make sure 2 appropriate corners are cut
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cornerStyle"></param>
    /// <param name="numberOfCornersToCut"></param>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    protected override void SetTypes(RoomStyle.RoomStyleType type, CutCornerStyle cornerStyle, int numberOfCornersToCut = -1, int cornerSizeX = -1, int cornerSizeY = -1) {
        if ((int)type == (-1)) {
            type = EnumExtensions.GetRandomEntry<RoomStyle.RoomStyleType>();
        }
        style = type;
        do {
            corners = EnumExtensions.GetRandomEntry<CutCornerStyle>();
        } while (corners == CutCornerStyle.None);
        cornerPlacements = new CornerPlacement[0];

        PickCorners(cornerSizeX, cornerSizeY);
    }

    /// <summary>
    /// Places doors, taking into account that 2 corners are cut away further than usual
    /// </summary>
    /// <param name="doors"></param>
    protected override void PlaceDoors(List<Room.Door> doors) {

        bool updown = false;
        if (existingCorners.Contains(CornerPlacement.TopLeft) && existingCorners.Contains(CornerPlacement.TopRight)) {
            updown = true;
        }
        if (existingCorners.Contains(CornerPlacement.BottomLeft) && existingCorners.Contains(CornerPlacement.BottomRight)) {
            updown = true;
        }

        foreach (Room.Door door in doors) {
            switch (door.Direction) {
                case Room.Direction.Up:
                    if (updown) {
                        if(layout[XLength / 2 - 1, 0] == null && layout[XLength / 2 + 1, 0] == null) {
                            layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                            continue;
                        }
                        if(layout[XLength / 2 - 1, 0] == null) {
                            layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                            continue;
                        }
                        if(layout[XLength / 2 + 1, 0] == null) {
                            layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                            continue;
                        }
                        layout[XLength / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Left, room, new Vector3(XLength / 2, 0, 0), style, door);
                        continue;
                    }
                    if (existingCorners.Contains(CornerPlacement.TopRight)) {
                        if(XLength - ((XLength - sizeOfCornersX) / 2) == XLength - 1) {
                            layout[XLength - ((XLength - sizeOfCornersX) / 2), 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Left, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, 0), style, door);
                            continue;
                        }
                        if(layout[XLength - ((XLength - sizeOfCornersX) / 2) - 1, 0] == null) {
                            layout[XLength - ((XLength - sizeOfCornersX) / 2), 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Left, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, 0), style, door);
                            continue;
                        }
                        layout[XLength - ((XLength - sizeOfCornersX) / 2), 0] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Left, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, 0), style, door);
                        continue;
                    }
                    if (((XLength - sizeOfCornersX) / 2) == 0) {
                        layout[(XLength - sizeOfCornersX) / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Left, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, 0), style, door);
                        continue;
                    }
                    if (layout[(XLength - sizeOfCornersX) / 2 + 1, 0] == null) {
                        layout[(XLength - sizeOfCornersX) / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Left, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, 0), style, door);
                        continue;
                    }
                    layout[(XLength - sizeOfCornersX) / 2, 0] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Left, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, 0), style, door);
                    continue;

                case Room.Direction.Right:
                    if (!updown) {
                        if (layout[XLength - 1, YLength / 2 - 1] == null && layout[XLength - 1, YLength / 2 + 1] == null) {
                            layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                            continue;
                        }
                        if (layout[XLength - 1, YLength / 2 - 1] == null) {
                            layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                            continue;
                        }
                        if (layout[XLength - 1, YLength / 2 + 1] == null) {
                            layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                            continue;
                        }
                        layout[XLength - 1, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength / 2), style, door);
                        continue;
                    }
                    if (existingCorners.Contains(CornerPlacement.TopRight)) {
                        if((YLength - sizeOfCornersY) / 2 == 0){
                            layout[XLength - 1, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, (YLength - sizeOfCornersY) / 2), style, door);
                            continue;
                        }
                        if (layout[XLength - 1, (YLength - sizeOfCornersY) / 2 + 1] == null) {
                            layout[XLength - 1, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, (YLength - sizeOfCornersY) / 2), style, door);
                            continue;
                        }
                        layout[XLength - 1, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, (YLength - sizeOfCornersY) / 2), style, door);
                        continue;
                    }

                    if (layout[XLength - 1, YLength - ((YLength - sizeOfCornersY) / 2)] == null) {
                        layout[XLength - 1, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, (YLength - sizeOfCornersY) / 2), style, door);
                        continue;
                    }
                    if (YLength - ((YLength - sizeOfCornersY) / 2) == YLength - 1) {
                        layout[XLength - 1, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength - ((YLength - sizeOfCornersY) / 2)), style, door);
                        continue;
                    }
                    layout[XLength - 1, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Down, room, new Vector3(XLength - 1, 0, YLength - ((YLength - sizeOfCornersY) / 2)), style, door);
                    continue;

                case Room.Direction.Down:
                    if (updown) {
                        if (layout[XLength / 2 - 1, YLength - 1] == null && layout[XLength / 2 + 1, 0] == null) {
                            layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                            continue;
                        }
                        if (layout[XLength / 2 - 1, YLength - 1] == null) {
                            layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                            continue;
                        }
                        if (layout[XLength / 2 + 1, YLength - 1] == null) {
                            layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                            continue;
                        }
                        layout[XLength / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Right, room, new Vector3(XLength / 2, 0, YLength - 1), style, door);
                        continue;
                    }
                    if (existingCorners.Contains(CornerPlacement.TopRight)) {
                        if(XLength - ((XLength - sizeOfCornersX) / 2) == XLength - 1) {
                            layout[XLength - ((XLength - sizeOfCornersX) / 2), YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Right, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, YLength - 1), style, door);
                            continue;
                        }
                        if (layout[XLength - ((XLength - sizeOfCornersX) / 2 + 1), YLength - 1] == null) {
                            layout[XLength - ((XLength - sizeOfCornersX) / 2), YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Right, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, YLength - 1), style, door);
                            continue;
                        }
                        layout[XLength - ((XLength - sizeOfCornersX) / 2), YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Right, room, new Vector3(XLength - ((XLength - sizeOfCornersX) / 2), 0, YLength - 1), style, door);
                        continue;
                    }
                    if (layout[(XLength - sizeOfCornersX) / 2, YLength - 1] == null) {
                        layout[(XLength - sizeOfCornersX) / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Right, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, YLength - 1), style, door);
                        continue;
                    }
                    if ((XLength - sizeOfCornersX) / 2 == 0) {
                        layout[XLength - ((XLength - sizeOfCornersX) / 2), YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Right, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, YLength - 1), style, door);
                        continue;
                    }
                    layout[(XLength - sizeOfCornersX) / 2, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Right, room, new Vector3((XLength - sizeOfCornersX) / 2, 0, YLength - 1), style, door);
                    
                    continue;

                case Room.Direction.Left:
                    if (!updown) {
                        if (layout[0, YLength / 2 - 1] == null && layout[0, YLength / 2 + 1] == null) {
                            layout[0, YLength / 2] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength / 2), style, door);
                            continue;
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
                    }
                    if (existingCorners.Contains(CornerPlacement.TopRight)) {
                        if((YLength - sizeOfCornersY) / 2 == 0) {
                            layout[0, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Up, room, new Vector3(0, 0, (YLength - sizeOfCornersY) / 2), style, door);
                            continue;
                        }
                        if(layout[0, (YLength - sizeOfCornersY) / 2] == null) {
                            layout[0, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Up, room, new Vector3(0, 0, (YLength - sizeOfCornersY) / 2), style, door);
                            continue;
                        }
                        layout[0, (YLength - sizeOfCornersY) / 2] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Up, room, new Vector3(0, 0, (YLength - sizeOfCornersY) / 2), style, door);
                        continue;
                    }
                    if (YLength - ((YLength - sizeOfCornersY) / 2) == YLength - 1) {
                        layout[0, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorLeft, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength - ((YLength - sizeOfCornersY) / 2)), style, door);
                        continue;
                    }
                    if (layout[0, YLength - ((YLength - sizeOfCornersY) / 2)] == null) {
                        layout[0, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.cornerDoorRight, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength - ((YLength - sizeOfCornersY) / 2)), style, door);
                        continue;
                    }
                    layout[0, YLength - ((YLength - sizeOfCornersY) / 2)] = new DoorPiece(RoomPiece.RoomPieceType.door, RoomPiece.Rotation.Up, room, new Vector3(0, 0, YLength - ((YLength - sizeOfCornersY) / 2)), style, door);
                    continue;

                default:
                    continue;

            }
        }
    }

    /// <summary>
    /// Picks the 2 corners to be cut away, making sure they're next to each other
    /// </summary>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    private void PickCorners(int cornerSizeX, int cornerSizeY) {

        if (layout.GetLength(0) >= layout.GetLength(1)) {
            sizeOfCornersX = (cornerSizeX == -1) ? UnityEngine.Random.Range(minCornerSize, (XLength - minSpaceAroundDoor) / 2) : cornerSizeX;
            sizeOfCornersY = (cornerSizeY == -1) ? UnityEngine.Random.Range(layout.GetLength(1) / 2, layout.GetLength(1) - minSpaceAroundDoor) : cornerSizeY;
            if (UnityEngine.Random.Range(0, 2) == 0) {
                existingCorners[0] = CornerPlacement.TopLeft;
                existingCorners[1] = CornerPlacement.TopRight;
            } else {
                existingCorners[0] = CornerPlacement.BottomLeft;
                existingCorners[1] = CornerPlacement.BottomRight;
            }
        } else {
            sizeOfCornersX = (cornerSizeX == -1) ? UnityEngine.Random.Range(layout.GetLength(0) / 2, layout.GetLength(0) - minSpaceAroundDoor) : cornerSizeX;
            sizeOfCornersY = (cornerSizeY == -1) ? UnityEngine.Random.Range(minCornerSize, (YLength - minSpaceAroundDoor) / 2) : cornerSizeY;
            if (UnityEngine.Random.Range(0, 2) == 0) {
                existingCorners[0] = CornerPlacement.TopLeft;
                existingCorners[1] = CornerPlacement.BottomLeft;
            } else {
                existingCorners[0] = CornerPlacement.TopRight;
                existingCorners[1] = CornerPlacement.BottomRight;
            }

        }

        CornerPlacement[] allCorners = EnumExtensions.GetDistinctRandomEntries<CornerPlacement>(Enum.GetNames(typeof(CornerPlacement)).Length);
        cornerPlacements = new CornerPlacement[2];
        int count = 0;
        foreach (CornerPlacement corner in allCorners) {
            if (!existingCorners.Contains(corner)) {
                cornerPlacements[count] = corner;
                count++;
                if (count >= 2) {
                    break;
                }
            }
        }
    }
}
