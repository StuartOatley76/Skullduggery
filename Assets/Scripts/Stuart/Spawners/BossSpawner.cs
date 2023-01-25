using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Subclass of EnemySpawner to handle spawning the boss
/// </summary>
public class BossSpawner : EnemySpawner
{
    /// <summary>
    /// Override to spawn boss in the centre of the room, or as close as possible on the navmesh
    /// </summary>
    /// <param name="information"></param>
    /// <returns></returns>
    protected override IEnumerator SpawnEnemies(RoomInformation information) {
        if(information.Style.BossPrefab == null) {
            yield break;
        }
        int framesWaited = 0;
        while (framesWaited < framesToWait) {
            yield return null;
            framesWaited++;
        }
        RoomNavmesh navmesh = GetComponent<RoomNavmesh>();
        if (navmesh != null) {
            navmesh.BuildNavMesh();
        }
        Vector3 spawnLocation = information.CentrePiece.transform.position;
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


        GameObject boss = Instantiate(information.Style.BossPrefab, spawnLocation, Quaternion.identity);
        information.ActiveEnemies = new List<GameObject>() { boss };
    }
}
