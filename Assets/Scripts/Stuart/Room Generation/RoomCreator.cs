using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Monobehaviour to trigger the creation of the level
/// </summary>
public class RoomCreator : MonoBehaviour
{
    /// <summary>
    /// minimum number of rooms on the main path through
    /// </summary>
    [SerializeField] private int minRoomsOnPath;

    /// <summary>
    /// Maximum number of rooms on the main path through
    /// </summary>
    [SerializeField] private int maxRoomsOnPath;

    /// <summary>
    /// How far it can go off of the main path
    /// </summary>
    [SerializeField] private int maxRoomsOnOffshoot;

    /// <summary>
    /// Triggers level creation
    /// </summary>
    void Start()
    {
        Room.CreateLevel(minRoomsOnPath, maxRoomsOnPath, maxRoomsOnOffshoot);
    }
}
