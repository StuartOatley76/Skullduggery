using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Spawner to handle spawning in the tutorial scene
/// </summary>
public class TutorialSpawner : EnemySpawner { 

    /// <summary>
    /// prefabs for the enemies in this room
    /// </summary>
    [SerializeField] GameObject[] enemyPrefabs;

    /// <summary>
    /// Spawn points for the enemies in here
    /// </summary>
    [SerializeField] GameObject[] spawnPoints;

    /// <summary>
    /// Starts the enemies spawning if not previously spawned
    /// </summary>
    public override void SpawnEnemies() {
        if (hasSpawned) {
            return;
        }
        StartCoroutine(SpawnTutorialEnemies());
    }

    /// <summary>
    /// Waits for setup to complete then spawns the enemies at the locations provided on the navmesh
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnTutorialEnemies() {
        List<GameObject> enemies = new List<GameObject>();
        int framesWaited = 0;
        while (framesWaited < framesToWait) {
            yield return null;
            framesWaited++;
        }
        RoomNavmesh navmesh = GetComponent<RoomNavmesh>();
        if (navmesh != null) {
            navmesh.BuildNavMesh();
        }
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            if (i == spawnPoints.Length) {
                break;
            }
            Vector3 spawnLocation = spawnPoints[i].transform.position;
            bool foundNavMesh = false;
            float range = 0.25f;
            NavMeshHit hit;
            do {
                if (NavMesh.SamplePosition(spawnLocation, out hit, range, 1)) {
                    spawnLocation = hit.position;
                    foundNavMesh = true;
                } else {
                    range += range;
                }
            } while (!foundNavMesh);

            enemies.Add(Instantiate(enemyPrefabs[i], spawnLocation, Quaternion.identity));
        }
        RoomInformation information = GetComponent<RoomInformation>();
        if (information) {
            information.ActiveEnemies = enemies;
        }
    }
}

