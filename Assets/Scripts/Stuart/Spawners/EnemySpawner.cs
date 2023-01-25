using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class to handle spawning Enemies, selecting enemies at random but with limitations
/// on the type and number based on how far through the level the player is and which level they are on
/// </summary>
public class EnemySpawner : MonoBehaviour
{

    /// <summary>
    /// How many rooms are on the main path 
    /// </summary>
    public static int NumberOfRoomsOnMainPath { private get; set; } = -1;
    /// <summary>
    /// The current active room
    /// </summary>
    private static int currentRoom = 0;

    /// <summary>
    /// The level the player is on
    /// </summary>
    public static int Level { get; set; } = 1;

    /// <summary>
    /// How many additional enemies for this level
    /// </summary>
    private static int newEnemyEveryXLevels = -1;

    /// <summary>
    /// Min number of enemies in a room
    /// </summary>
    private static int minimumNumberOfenemiesInRoom = 2;

    /// <summary>
    /// Max number of enemies in a room - increases on higher levels
    /// </summary>
    private static int maxNumberOfEnemies = 7;

    /// <summary>
    /// All the enemies
    /// </summary>
    private List<GameObject> RoomEnemies;

    /// <summary>
    /// Whether this spawner has already spawned
    /// </summary>
    protected bool hasSpawned = false;

    /// <summary>
    /// positions to spawn the enemies
    /// </summary>
    private List<Vector3> spawnPoints = new List<Vector3>();

    /// <summary>
    /// minimum spawn distance from each other
    /// </summary>
    private float minSpawnDistanceApart = 0.5f;

    /// <summary>
    /// minimum spawn distance from the player
    /// </summary>
    private float minSpawnDistanceFromPlayer = 1f * RoomPiece.ModelScale;

    /// <summary>
    /// lenelay to allow everything to
    /// </summary>
    [SerializeField] protected int framesToWait = 6;

    /// <summary>
    /// how many rooms have previously been entered
    /// </summary>
    private static int previousRooms = 0;

    /// <summary>
    /// Index in the list of enemies for ghosts to
    /// prevent ghost only rooms
    /// </summary>
    private static int ghostIndex = 2;


    private void Awake() {
        //CharacterController.PlayerKilled += Reset;
        Reset(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets everything after the player dies
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private static void Reset(object o, EventArgs e) {
        minimumNumberOfenemiesInRoom = 2;
        previousRooms = 0;
        NumberOfRoomsOnMainPath = -1;
        currentRoom = 0;
        newEnemyEveryXLevels = -1;
        previousRooms = 0;
        maxNumberOfEnemies = 7;
        CharacterController.PlayerKilled -= Reset;
    }

    /// <summary>
    /// Prepares everything to spawn enemies
    /// </summary>
    public virtual void SpawnEnemies() {
        if (hasSpawned) {
            return;
        }
        BaseEnemy.enemyCounter = 0;
        RoomInformation information = GetComponent<RoomInformation>();
        if (information == null) {
            return;
        }
        if (RoomEnemies == null) {
            RoomEnemies = information.GetPossibleEnemies();
        }
        StartCoroutine(SpawnEnemies(information));

    }

    /// <summary>
    /// Gives time for everything in the level to be set up, ensures the traps and navmesh are in place before spawning
    /// </summary>
    /// <param name="information"></param>
    /// <returns></returns>
    protected virtual IEnumerator SpawnEnemies(RoomInformation information) {
        int framesWaited = 0;
        while(framesWaited < framesToWait) {
            yield return null;
            framesWaited++;
        }

        yield return new WaitUntil(() => PlayerSpawner.instance.Player != null);

        if (information.Type != Room.RoomType.Challange) {
            currentRoom++;
        }
        Bounds roomBounds = information.GetRoomBounds();
        TrapSpawner.SpawnTraps(gameObject, currentRoom, NumberOfRoomsOnMainPath);
        RoomNavmesh navmesh = GetComponent<RoomNavmesh>();
        if(navmesh != null) {
            navmesh.BuildNavMesh();
        }
        information.ActiveEnemies = Spawn(roomBounds, information.Type);
        
    }

    /// <summary>
    /// Triggers the creation of enemies based on the level and progression through the level
    /// </summary>
    /// <param name="roomBounds"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<GameObject> Spawn(Bounds roomBounds, Room.RoomType type) {
        if(newEnemyEveryXLevels == -1) {
            CalculateNewEnemy();
        }
        int maxEnemy = previousRooms + currentRoom - 1;
        maxEnemy = (maxEnemy > RoomEnemies.Count - 1) ? RoomEnemies.Count - 1 : (maxEnemy < 0) ? 0 : maxEnemy;
        int numberOfEnemies = minimumNumberOfenemiesInRoom + previousRooms + currentRoom - 1;
        numberOfEnemies = (numberOfEnemies > maxNumberOfEnemies) ? maxNumberOfEnemies : numberOfEnemies;
        if(type == Room.RoomType.Challange) {
            numberOfEnemies += 3;
            numberOfEnemies = (numberOfEnemies > NumberOfRoomsOnMainPath) ? NumberOfRoomsOnMainPath : numberOfEnemies;
            maxEnemy = RoomEnemies.Count - 1;
        }
        List<GameObject> enemies = CreateEnemies(numberOfEnemies, maxEnemy, roomBounds);
        hasSpawned = true;
        return enemies;
    }

    /// <summary>
    /// Randomly chooses and instantiates enemies based on the limitations from Spawn
    /// </summary>
    /// <param name="numberOfEnemies"></param>
    /// <param name="maxEnemy"></param>
    /// <param name="roomBounds"></param>
    /// <returns></returns>
    private List<GameObject> CreateEnemies(int numberOfEnemies, int maxEnemy, Bounds roomBounds) {
        List<GameObject> enemies = new List<GameObject>();
        Vector3 position = GetSpawnPoint(roomBounds);
        enemies.Add(Instantiate(RoomEnemies[maxEnemy], position, Quaternion.identity));
        numberOfEnemies--;
        for(int i = 0; i < numberOfEnemies; i++) {

            int enemyType;
            if (i + 1 == numberOfEnemies && enemies.All(e => e.GetComponent<PacMan>() != null)) {

                do {
                    enemyType = UnityEngine.Random.Range(0, maxEnemy + 1);
                } while (enemyType == ghostIndex);
            } else {
                enemyType = UnityEngine.Random.Range(0, maxEnemy + 1);
            }
            Vector3 pos = GetSpawnPoint(roomBounds);
            enemies.Add(Instantiate(RoomEnemies[enemyType], pos, Quaternion.identity));
        }
        return enemies;
    }

    /// <summary>
    /// Finds spawn points for the enemies
    /// </summary>
    /// <param name="roomBounds"></param>
    /// <returns></returns>
    private Vector3 GetSpawnPoint(Bounds roomBounds) {
        int safety = 0;
        NavMeshHit hit;
        RoomInformation information = GetComponent<RoomInformation>();
        do {
            Vector3 position = new Vector3(
                UnityEngine.Random.Range(roomBounds.min.x, roomBounds.max.x),
                0,
                UnityEngine.Random.Range(roomBounds.min.z, roomBounds.max.z)
                );
            bool toClose = false;
            if(Vector3.Distance(position, PlayerSpawner.instance.Player.transform.position) < minSpawnDistanceFromPlayer) {
                toClose = true;
            } else { 
                foreach(Vector3 currentPos in spawnPoints) {
                    if(Vector3.Distance(position, currentPos) < minSpawnDistanceApart) {
                        toClose = true;
                    }
                }
            }
            Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
            if(colliders.Length < 0) {
                toClose = true;
            }
            bool foundFloor = false;
            foreach (Collider collider in colliders) {
                if (collider.gameObject.transform.parent != null && information.FloorPieces.Contains(collider.gameObject.transform.parent.gameObject)) {
                    foundFloor = true;
                }
            }
            toClose = (!foundFloor) ? true : toClose;

            if (!toClose) {
                if(!NavMesh.SamplePosition(position, out hit, 5f, 1)) {
                    continue;
                }
                spawnPoints.Add(hit.position);
                return hit.position;
            }
            safety++;
        } while (safety < 1000000);

        Debug.LogError("No valid spawn point found");

        NavMesh.SamplePosition(Vector3.zero, out hit, 25f, 1);
        spawnPoints.Add(hit.position);
        return hit.position;
    }

    /// <summary>
    /// calculates how often the number of enemies should increase based on the length of the path through the level
    /// </summary>
    private void CalculateNewEnemy() {
        newEnemyEveryXLevels = NumberOfRoomsOnMainPath / RoomEnemies.Count;
    }

    /// <summary>
    /// Increases the minimum and maximum number of enemies for the next level
    /// </summary>
    public static void NewLevel() {
        //minimumNumberOfenemiesInRoom += minimumNumberOfenemiesInRoom + 1;
        //maxNumberOfEnemies += maxNumberOfEnemies + 1;
    }
}
