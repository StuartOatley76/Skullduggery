using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold information about a piece of a room
/// </summary>
public class RoomPiece
{
    /// <summary>
    /// Enum to represent the possible types of room piece
    /// </summary>
    public enum RoomPieceType {
        none = 0,
        floor,
        wall,
        pillar,
        door,
        corner,
        cornerDoorLeft,
        cornerDoorRight,
        innerCorner,
        corridorEnd,
        corridorEndDoor,
        corridorPiece,
        block,
        emptyFloor,
    }

    /// <summary>
    /// enum to represent the rotation of the room piece
    /// </summary>
    public enum Rotation {
        Left = 0,
        Up = 90,
        Right = 180,
        Down = 270
    }

    /// <summary>
    /// Position scaled to model size
    /// </summary>
    private Vector3 position;
    public Vector3 Position {
        get { return position; } 
        set { position = value * ModelScale; } 
    }

    /// <summary>
    /// Rotation of the piece
    /// </summary>
    public Rotation PieceRotation { get; set; } = Rotation.Left;

    /// <summary>
    /// Type of the piece
    /// </summary>
    public RoomPieceType PieceType { get; set; }

    /// <summary>
    /// Style of the piece
    /// </summary>
    public RoomStyle.RoomStyleType PieceStyle { get; private set; }

    /// <summary>
    /// prefab of the piece
    /// </summary>
    private GameObject piecePrefab;

    /// <summary>
    /// Gets the prefab for the piece
    /// </summary>
    /// <returns></returns>
    public GameObject GetPiecePrefab() {
        if (piecePrefab == null) {
            piecePrefab = AllRoomStyles.GetStyle(PieceStyle).GetObject(PieceType);
        }
        return piecePrefab;
    }

    /// <summary>
    /// Instance of the piece
    /// </summary>
    public virtual GameObject PieceInstance { get; set; }

    /// <summary>
    /// Gameobject the piece is in
    /// </summary>
    public GameObject RoomHolder { get; set; }

    /// <summary>
    /// The model scale
    /// </summary>
    public static int ModelScale { get; private set; } = 5;

    /// <summary>
    /// Sets the appropriate material on the piece
    /// </summary>
    /// <param name="go"></param>
    /// <param name="type"></param>
    public void SetMaterial(GameObject go, Room.RoomType type) {
        AllRoomStyles.GetStyle(PieceStyle).SetMaterial(go, type);
    }

    /// <summary>
    /// Constructor. Initialises variables
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rotation"></param>
    /// <param name="room"></param>
    /// <param name="positionInRoom"></param>
    /// <param name="style"></param>
    public RoomPiece(RoomPieceType type, Rotation rotation, GameObject room, Vector3 positionInRoom, RoomStyle.RoomStyleType style) {
        PieceType = type;
        PieceRotation = rotation;
        if(type == RoomPieceType.floor) {
            PieceRotation = EnumExtensions.GetRandomEntry<Rotation>();
        }
        RoomHolder = room;
        Position = positionInRoom;
        PieceStyle = style;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="piece"></param>
    public RoomPiece(RoomPiece piece) {
        PieceType = piece.PieceType;
        PieceRotation = piece.PieceRotation;
        RoomHolder = piece.RoomHolder;
        position = piece.position;
        PieceStyle = piece.PieceStyle;
    }

    /// <summary>
    /// rotates the rotation of the piece
    /// </summary>
    /// <param name="numberOf90DegreeRotations"></param>
    public void RotatePiece(int numberOf90DegreeRotations) {
        int rotation = (int)PieceRotation;
        rotation += numberOf90DegreeRotations;
        if(rotation < 0) {
            rotation += 360;
        }
        if(rotation > 270) {
            rotation -= 360;
        }
        PieceRotation = (Rotation)rotation;
    }

    /// <summary>
    /// Applies the rotation to the gameobject
    /// </summary>
    /// <param name="piece"></param>
    public void SetRotation(GameObject piece) {
        piece.transform.Rotate(new Vector3(0, (int)PieceRotation, 0), Space.Self);
    }
}
