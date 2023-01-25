using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle getting the right style of piece.
/// Originally created early when we hoped to get more than just the arcade 
/// done. A lot was built around it, so needs to stay in even with just one
/// </summary>
public class AllRoomStyles : MonoBehaviour
{
    /// <summary>
    /// Instance
    /// </summary>
    private static AllRoomStyles roomStyles;

    /// <summary>
    /// Setup
    /// </summary>
    private void Awake() {
        if(roomStyles != null) {
            Destroy(gameObject);
        }
        roomStyles = this;
    }

    /// <summary>
    /// List of the styles
    /// </summary>
    [SerializeField] private List<RoomStyle> allRoomStyles;

    /// <summary>
    /// Gets the style based on the type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static RoomStyle GetStyle(RoomStyle.RoomStyleType type) {
        return GetRoomStyle(type);
    }

    /// <summary>
    /// picks a random style
    /// </summary>
    /// <returns></returns>
    public static RoomStyle GetRandomStyle() {
        return roomStyles.allRoomStyles[Random.Range(0, roomStyles.allRoomStyles.Count)];
    }

    //Gets a specific style
    public static RoomStyle GetRoomStyle(RoomStyle.RoomStyleType type) {
        return roomStyles.allRoomStyles.Where(r => r.Type == type).First();
    }
}
