using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class to hold all peice prefabs for a style. Supplies random appropriate prefab
/// </summary>
public class RoomStyle : MonoBehaviour
{

    /// <summary>
    /// enum to represent room styles
    /// </summary>
    public enum RoomStyleType {
        Arcade,
    }

    /// <summary>
    /// Enum to represent different material types
    /// </summary>
    private enum matType {
        wall,
        floor,
        pillar
    }
    /// <summary>
    /// This style
    /// </summary>
    [SerializeField] private RoomStyleType type;
    public RoomStyleType Type { get { return type; } }

    //Lists of all the room piece prefabs for the style

    [SerializeField] private List<GameObject> floorPieces;
    [SerializeField] private List<GameObject> wallPieces;
    [SerializeField] private List<GameObject> cornerPieces;
    [SerializeField] private List<GameObject> pillarPieces;
    [SerializeField] private List<GameObject> doorPieces;
    [SerializeField] private List<GameObject> cornerDoorLeftPieces;
    [SerializeField] private List<GameObject> cornerDoorRightPieces;
    [SerializeField] private List<GameObject> innerCornerPieces;
    [SerializeField] private List<GameObject> corridorEnds;
    [SerializeField] private List<GameObject> corridorEndDoors;
    [SerializeField] private List<GameObject> corridorPieces;
    [SerializeField] private List<GameObject> blockPieces;

    /// <summary>
    /// Boss room prefab
    /// </summary>
    [SerializeField] private GameObject bossRoom;

    /// <summary>
    /// list of possible enemies foir the style
    /// </summary>
    [SerializeField] private List<GameObject> styleEnemies;

    /// <summary>
    /// The style's boss
    /// </summary>
    [SerializeField] private GameObject boss;

    /// <summary>
    /// The prefab for the dropped powerup
    /// </summary>
    [SerializeField] private GameObject powerUpPrefab;

    /// <summary>
    /// List of prefabs for the style
    /// </summary>
    [SerializeField] private List<GameObject> trapPrefabs;

    //The different materials for the style

    [SerializeField] private Material wallStandardMat;
    [SerializeField] private Material wallChallangeMat;
    [SerializeField] private Material wallBossMat;
    [SerializeField] private Material floorStandardMat;
    [SerializeField] private Material floorChallangeMat;
    [SerializeField] private Material floorBossMat;
    [SerializeField] private Material pillarStandardMat;
    [SerializeField] private Material pillarChallangeMat;
    [SerializeField] private Material pillarBossMat;

    //Lists to hold the materials

    private List<Material> wallMats = new List<Material>();
    private List<Material> floorMats = new List<Material>();
    private List<Material> pillarMats = new List<Material>();

    /// <summary>
    /// How many pieces have been supplied
    /// </summary>
    public static int PiecesGot = 0;

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake() {
        SetLists();
    }

    /// <summary>
    /// Puts the materials into their lists
    /// </summary>
    private void SetLists() {
        if(wallMats.Count == 0) {
            wallMats.Add(wallStandardMat);
            wallMats.Add(wallChallangeMat);
            wallMats.Add(wallBossMat);
        }
        if(floorMats.Count == 0) {
            floorMats.Add(floorStandardMat);
            floorMats.Add(floorChallangeMat);
            floorMats.Add(floorBossMat);
        }
        if(pillarMats.Count == 0) {
            pillarMats.Add(pillarStandardMat);
            pillarMats.Add(pillarChallangeMat);
            pillarMats.Add(pillarBossMat);
        }
    }

    /// <summary>
    /// Sets the appropriate material on each renderer on the supplied game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="roomType"></param>
    public void SetMaterial(GameObject gameObject, Room.RoomType roomType) {
        foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
            renderer.material = GetMat(roomType, renderer.sharedMaterial);
        }
    }

    /// <summary>
    /// returns the trap prefabs
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetTrapPrefabs() {
        return trapPrefabs;
    }

    /// <summary>
    /// Gets a floor piece withut any decoration
    /// </summary>
    /// <returns></returns>
    private GameObject GetEmptyFloorPiece() {
        return floorPieces[0];
    }

    /// <summary>
    /// Gets the appripriate replacement for a material based on room type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    private Material GetMat(Room.RoomType type, Material material) {

        SetLists();

        if (wallMats.Contains(material)) {
        return GetMaterial(type, matType.wall);
        }
        if (floorMats.Contains(material)) {
        return GetMaterial(type, matType.floor);
        }
        if (pillarMats.Contains(material)) {
        return GetMaterial(type, matType.pillar);
        }
        return material;
    }

    /// <summary>
    /// Gets the material based on room type and mat type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="matType"></param>
    /// <returns></returns>
    private Material GetMaterial(Room.RoomType type, matType matType) {
        switch (type) {
            case Room.RoomType.MainPath:
            case Room.RoomType.Starting:
                if(matType == matType.wall) {
                    return wallStandardMat;
                }
                if(matType == matType.floor) {
                    return floorStandardMat;
                }
                if(matType == matType.pillar) {
                    return pillarStandardMat;
                }
                break;

            case Room.RoomType.Challange:
                if (matType == matType.wall) {
                    return wallChallangeMat;
                }
                if (matType == matType.floor) {
                    return floorChallangeMat;
                }
                if (matType == matType.pillar) {
                    return pillarChallangeMat;
                }
                break;

            case Room.RoomType.Boss:
                if (matType == matType.wall) {
                    return wallBossMat;
                }
                if (matType == matType.floor) {
                    return floorBossMat;
                }
                if (matType == matType.pillar) {
                    return pillarBossMat;
                }
                break;
            default:
                break;
        }
        return null;
    }

    /// <summary>
    /// Getter for power up prefab
    /// </summary>
    public GameObject PowerUpPrefab { get { return powerUpPrefab; } }

    /// <summary>
    /// Weight for selection of a prefab without obsticles/decorations
    /// </summary>
    [Range(1, 100)]
    [SerializeField] private int clearWeight;

    /// <summary>
    /// Getter for style enemies
    /// </summary>
    public List<GameObject> StyleEnemies { get { return styleEnemies; } }

    //Door and sign materials

    [SerializeField] private Material mainPathDoorMat;
    public Material MainPathDoorMat { get { return mainPathDoorMat; } }

    [SerializeField] private Material mainPathSignMat;

    public Material MainPathSignMat { get { return mainPathSignMat; } }

    [SerializeField] private Material challangeRoomDoorMat;
    public Material ChallangeRoomDoorMat { get { return challangeRoomDoorMat; } }

    [SerializeField] private Material challangeRoomSignMat;

    public Material ChallangeRoomSignMat { get { return challangeRoomSignMat; } }

    [SerializeField] private Material bossRoomDoorMat;
    public Material BossRoomDoorMat { get { return bossRoomDoorMat; } }

    [SerializeField] private Material bossRoomSignMat;

    public Material BossRoomSignMat { get { return bossRoomSignMat; } }

    [SerializeField] private Material nextLevelDoorMat;

    public Material NextLevelDoorMat { get { return nextLevelDoorMat; } }

    [SerializeField] private Material nextLevelSignMat;

    public Material NextLevelSignMat { get { return nextLevelSignMat; } }

    /// <summary>
    /// getter for boss room prefab
    /// </summary>
    public GameObject BossRoomPrefab { get { return bossRoom; } }

    /// <summary>
    /// getter for boss prefab
    /// </summary>
    public GameObject BossPrefab { get { return boss; } }


    /// <summary>
    /// Passes the appropriate list of pieces into GetWeightedPieces to select a Piece
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public GameObject GetObject(RoomPiece.RoomPieceType piece) {
        switch (piece) {
            case RoomPiece.RoomPieceType.corner:
                return GetWeightedPiece(cornerPieces);
            case RoomPiece.RoomPieceType.wall:
                return  GetWeightedPiece(wallPieces);
            case RoomPiece.RoomPieceType.pillar:
                return GetWeightedPiece(pillarPieces);
            case RoomPiece.RoomPieceType.floor:
                return GetWeightedPiece(floorPieces);
            case RoomPiece.RoomPieceType.door:
                return GetWeightedPiece(doorPieces);
            case RoomPiece.RoomPieceType.innerCorner:
                return GetWeightedPiece(innerCornerPieces);
            case RoomPiece.RoomPieceType.cornerDoorLeft:
                return GetWeightedPiece(cornerDoorLeftPieces);
            case RoomPiece.RoomPieceType.cornerDoorRight:
                return GetWeightedPiece(cornerDoorRightPieces);
            case RoomPiece.RoomPieceType.corridorPiece:
                return GetWeightedPiece(corridorPieces);
            case RoomPiece.RoomPieceType.corridorEnd:
                return GetWeightedPiece(corridorEnds);
            case RoomPiece.RoomPieceType.corridorEndDoor:
                return GetWeightedPiece(corridorEndDoors);
            case RoomPiece.RoomPieceType.block:
                return GetWeightedPiece(blockPieces);
            case RoomPiece.RoomPieceType.emptyFloor:
                return GetEmptyFloorPiece();
            default:
                return GetEmptyFloorPiece();
        }
    }

    /// <summary>
    /// Selects a prefab taking into account the weighting for clear pieces
    /// </summary>
    /// <param name="pieces"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    private GameObject GetWeightedPiece(List<GameObject> pieces, bool debug = false) {

        if(Random.Range(0, 101) <= clearWeight || pieces.Count == 1) {
            return pieces[0];
        }
        GameObject pieceToReturn = pieces[Random.Range(1, pieces.Count)];
        
        return pieceToReturn;
    }

}
