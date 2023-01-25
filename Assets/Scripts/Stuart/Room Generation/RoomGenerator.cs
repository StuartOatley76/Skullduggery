using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

/// <summary>
/// Class to turn roompieces into gameobjects
/// </summary>
public class RoomGenerator : MonoBehaviour
{

    /// <summary>
    /// 2d array of the gameobjects created
    /// </summary>
    private GameObject[,] gameObjects;

    
    /// <summary>
    /// Triggers finding the appropriate prefab for each roompiece and instantiating it
    /// </summary>
    /// <param name="layout"></param>
    public void GenerateRoom(RoomPiece[,] layout) {
        gameObjects = new GameObject[layout.GetLength(0), layout.GetLength(1)];
        RoomInformation information = GetComponent<RoomInformation>();
        for(int x = 0; x < layout.GetLength(0); x++) {
            for(int y = 0; y < layout.GetLength(1); y++) {
                if(layout[x, y] == null) {
                    continue;
                }
                RoomPiece piece = layout[x, y];
                GameObject prefab = piece.GetPiecePrefab();
                GameObject pieceGO = Instantiate(prefab, transform, false);
                piece.SetMaterial(pieceGO, information.Type);
                pieceGO.SetActive(true);
                pieceGO.transform.position = gameObject.transform.position;
                pieceGO.transform.localPosition = piece.Position;
                piece.SetRotation(pieceGO);
                gameObjects[x, y] = pieceGO;
                if(x == layout.GetLength(0) / 2 &&  y == layout.GetLength(1) / 2) {
                    information.centrePoint = transform.TransformPoint(pieceGO.transform.localPosition);
                }
                if(piece.PieceType == RoomPiece.RoomPieceType.floor) {
                    information.FloorPieces.Add(pieceGO);
                }
                piece.PieceInstance = pieceGO;
            }
        }

    }

    /// <summary>
    /// Gets a gameobject's local position in the room from x and y position in the layout
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetPiecePosition(int x, int y) {
        if(gameObjects == null) {
            return Vector3.zero;
        }
        if(gameObjects[x, y] == null) {
            return Vector3.zero;
        }
        return transform.TransformPoint(gameObjects[x, y].transform.localPosition);
    }

    /// <summary>
    /// Destroys the room ready for the next level
    /// </summary>
    public void NewLevel() {
        foreach(GameObject go in gameObjects) {
            Destroy(go);
        }
        Destroy(gameObject);
    }
}
