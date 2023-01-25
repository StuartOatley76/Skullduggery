using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subclass of rectangular room layout for a square room
/// </summary>
public class SquareRoomLayout : RectangularRoomLayout {

    /// <summary>
    /// Constrctor. Exacly same as rectangular room but makes x and y the same lengthh
    /// </summary>
    /// <param name="doors"></param>
    /// <param name="type"></param>
    /// <param name="mapGO"></param>
    /// <param name="size"></param>
    /// <param name="style"></param>
    /// <param name="cornerStyle"></param>
    /// <param name="numberOfCornersToCut"></param>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    public SquareRoomLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO, int size, RoomStyle.RoomStyleType style, CutCornerStyle cornerStyle = (CutCornerStyle)(-1), int numberOfCornersToCut = -1,
        int cornerSizeX = -1, int cornerSizeY = -1) : base(doors, type, mapGO, size, size, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY) {

    }
   
}
