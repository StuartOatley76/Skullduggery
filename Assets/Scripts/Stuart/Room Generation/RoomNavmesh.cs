using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;


/// <summary>
/// Class to handle procedurally generating the navmesh for a room
/// Uses Unity's experimental AI navmesh package
/// </summary>
public class RoomNavmesh : MonoBehaviour {

    /// <summary>
    /// The mesh data created by the objects in the room
    /// </summary>
    private NavMeshData meshData;
    /// <summary>
    /// The sources og the meshdata
    /// </summary>
    private List<NavMeshBuildSource> navMeshBuildSources = new List<NavMeshBuildSource>();

    /// <summary>
    /// The navmesh instance created
    /// </summary>
    NavMeshDataInstance navmesh;

    /// <summary>
    /// What will be used to create the navmesh data
    /// </summary>
    private NavMeshSurface surface;

    /// <summary>
    /// whether the navmesh has been built
    /// </summary>
    private bool isBuilt = false;


    /// <summary>
    /// Collects all the sources, sets relevant information, then builds the navmesh
    /// </summary>
    public void BuildNavMesh() {
        if (isBuilt) {
            return;
        }
        surface = gameObject.AddComponent<NavMeshSurface>();
        surface.voxelSize = 0.125f;
        surface.collectObjects = CollectObjects.Children;
        meshData = new NavMeshData();
        navmesh = NavMesh.AddNavMeshData(meshData);
        RoomInformation information = GetComponent<RoomInformation>();
        if(information == null) {
            return;
        }
        Bounds navmeshBounds = information.GetRoomBounds();
        List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
        List<NavMeshModifier> modifiers = new List<NavMeshModifier>(surface.GetComponentsInChildren<NavMeshModifier>());
        foreach(NavMeshModifier modifier in modifiers) {
            markups.Add(new NavMeshBuildMarkup() {
                root = modifier.transform,
                overrideArea = modifier.overrideArea,
                area = modifier.area,
                ignoreFromBuild = modifier.ignoreFromBuild
            });
        }
        NavMeshBuilder.CollectSources(surface.transform, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, navMeshBuildSources);
        NavMeshBuilder.UpdateNavMeshData(meshData, surface.GetBuildSettings(), navMeshBuildSources, navmeshBounds);
        isBuilt = true;
    }

    /// <summary>
    /// Ensures the navmesh is destroyed
    /// </summary>
    private void OnDestroy() {
        NavMesh.RemoveNavMeshData(navmesh);
    }
}
