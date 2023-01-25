using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold and supply information about a room needed during gameplay
/// </summary>
public class RoomInformation : MonoBehaviour
{
    /// <summary>
    /// Static ref to the roominformation on the room the player is currently in
    /// </summary>
    public static RoomInformation activeRoom;

    /// <summary>
    /// The room type (starting, main path, challange, boss)
    /// </summary>
    [SerializeField] private Room.RoomType type;

    /// <summary>
    /// Event for when the boss room is entered
    /// </summary>
    public static EventHandler OnBossRoomEnter;

    /// <summary>
    /// Getter for room type
    /// </summary>
    public Room.RoomType Type { get { return type; } set { type = value; } }

    /// <summary>
    /// Style of the room
    /// </summary>
    [SerializeField] private RoomStyle style;
    public RoomStyle Style { get { return style; } set { style = value; } }

    /// <summary>
    /// Size of the room on the x axis
    /// </summary>
    [SerializeField] private float xSize;

    /// <summary>
    /// Size of the room on the x axis in world scale
    /// </summary>
    public float XSize { get { return xSize; } set { xSize = value * RoomPiece.ModelScale; } }


    /// <summary>
    /// Size of the room on the z axis
    /// </summary>
    [SerializeField] private float zSize;

    /// <summary>
    /// Size of the room on the z axis in world scale
    /// </summary>
    public float ZSize { get { return zSize; } set { zSize = value * RoomPiece.ModelScale; } }

    /// <summary>
    /// Centre of the room
    /// </summary>
    [SerializeField] public Vector3 centrePoint { get; set; }

    /// <summary>
    /// Game object at the centre of the room
    /// </summary>

    [SerializeField] private GameObject centrepeice;
    public GameObject CentrePiece { get { return centrepeice; } }

    /// <summary>
    /// Minimap object for the room
    /// </summary>
    public GameObject MapObject { get; set; }

    /// <summary>
    /// Size of the room on the y axis
    /// </summary>
    private float ySize = 10;

    /// <summary>
    /// List of all the floor pieces in the room
    /// </summary>
    [SerializeField] private List<GameObject> floorPieces = new List<GameObject>();
    public List<GameObject> FloorPieces { get { return floorPieces; } }

    /// <summary>
    /// List of the active enemies in the room
    /// </summary>
    private List<GameObject> activeEnemies;

    /// <summary>
    /// Event for when the room is cleared of enemies
    /// </summary>
    public static EventHandler RoomCleared;

    /// <summary>
    /// setter for the active enemies, sets room as active and if boss room triggers event
    /// </summary>
    public List<GameObject> ActiveEnemies { 
        set { 
            activeEnemies = value;
            activeRoom = this;
            if(Type == Room.RoomType.Boss) {
                OnBossRoomEnter?.Invoke(this, EventArgs.Empty);
            }
        } 
    }

    /// <summary>
    /// Gets the bounds of the room
    /// </summary>
    /// <returns></returns>
    public Bounds GetRoomBounds() {
        return new Bounds(centrePoint, new Vector3(xSize, ySize, zSize));
    }

    /// <summary>
    /// Gets the possible enemies that can spawn based on the style
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetPossibleEnemies() {
        return style.StyleEnemies;
    }

    /// <summary>
    /// Checks if the room was cleared and if so invokes the event, triggers the doors to open, and if challange room, drops 
    /// power up
    /// </summary>
    /// <param name="enemy"></param>
    public void EnemyKilled(GameObject enemy) {
        if (activeEnemies != null) {
            activeEnemies.Remove(enemy);
        }
        if(BaseEnemy.enemyCounter == 0) {
            RoomCleared?.Invoke(this, EventArgs.Empty);
            Teleporter[] doors = GetComponentsInChildren<Teleporter>();
            foreach(Teleporter door in doors) {
                door.Usable = true;
            }
            TeleporterToGame[] DoorsOut = GetComponentsInChildren<TeleporterToGame>();
            foreach(TeleporterToGame doorOut in DoorsOut) {
                doorOut.Usable = true;
            }
            if (Type == Room.RoomType.Challange) {
                Instantiate(AllRoomStyles.GetStyle(style.Type).PowerUpPrefab, enemy.transform.position, Quaternion.identity);
            }
        }
    }
}
