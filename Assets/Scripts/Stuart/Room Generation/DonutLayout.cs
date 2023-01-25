using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subclass of rectangular room to handle room with hole in the middle
/// </summary>
public class DonutLayout : RectangularRoomLayout {
    private int minPathWidth = 2;
    private int maxPathWidth = 2;
    public DonutLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO, int xSize, int ySize, RoomStyle.RoomStyleType style) :
        base(doors, type, mapGO, xSize, ySize, style, CutCornerStyle.None, 0, 0, 0) {
    }

    /// <summary>
    /// calls rectangular room's SetupLayout Then creates a random sized hole in the middle ensuring 
    /// enough space left at the edge of the room
    /// </summary>
    protected override void SetupLayout() {
        base.SetupLayout();
        int pathWidth = Random.Range(minPathWidth, maxPathWidth + 1);
        int centreWidth = XLength -  2 * pathWidth;
        int centreDepth = YLength - 2 * pathWidth;

        for(int x = pathWidth; x < pathWidth + centreWidth; x++) {
            for(int y = pathWidth; y < pathWidth + centreDepth; y++) {
                layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.block, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
            }
        }
    }

}
