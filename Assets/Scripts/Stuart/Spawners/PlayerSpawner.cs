using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class to handle spawning the player
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// The instance of the player spawner
    /// </summary>
    public static PlayerSpawner instance;

    /// <summary>
    /// How many frames to wait until spawning the player
    /// </summary>
    [SerializeField] private int framesToWait = 3;

    /// <summary>
    /// The player prefab
    /// </summary>
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// The camera used to view the room assembly
    /// </summary>
    private static Camera roomCamera;

    /// <summary>
    /// The player
    /// </summary>
    public GameObject Player { get; private set; }

    /// <summary>
    /// Height to spawn the player
    /// </summary>
    [SerializeField]
    private float ySpawnPosition = 2.65f;

    /// <summary>
    /// Sets up the instance
    /// </summary>
    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts the splayer spawning
    /// </summary>
    /// <param name="positionToSpawn"></param>
    public void SpawnPlayer(Vector3 positionToSpawn) {
        StartCoroutine(PlayerSpawn(positionToSpawn));
    }

    /// <summary>
    /// waits until everything is ready, then finds a spot on the navmesh to spawn the player
    /// </summary>
    /// <param name="positionToSpawn"></param>
    /// <returns></returns>
    private IEnumerator PlayerSpawn(Vector3 positionToSpawn) {
        int framesWaited = 0;
        while (framesWaited < framesToWait) {
            yield return null;
            framesWaited++;
        }

        yield return WaitForRoomAssembly();
        float range = 0.5f;
        NavMeshHit hit;
        int safety = 0;
        while (!NavMesh.SamplePosition(positionToSpawn, out hit, range, 1) && safety < 10000) {
            range += range;
            safety += 1;
        }
        if(safety >= 10000) {
            yield break;
        }
        Player = Instantiate(playerPrefab, new Vector3(hit.position.x, ySpawnPosition, hit.position.z), Quaternion.identity);
        CharacterController character = Player.GetComponent<CharacterController>();
        CameraFade cameraFade = FindObjectOfType<CameraFade>();
        if(cameraFade && cameraFade.IsFaded) {
            cameraFade.ToggleFade();
        }
    }

    private IEnumerator WaitForRoomAssembly() {

        if(GameObject.FindGameObjectWithTag("Starting Room") == null) {
            yield break;
        }

        ExplodeRoom explodeRoom = GameObject.FindGameObjectWithTag("Starting Room").GetComponent<ExplodeRoom>();
        if (roomCamera == null) {
            roomCamera = GameObject.FindGameObjectWithTag("RoomCreationCamera").GetComponent<Camera>();
        }
        if (explodeRoom != null && roomCamera != null && !explodeRoom.HasBeenAssembled) {
            roomCamera.transform.position = new Vector3(
                explodeRoom.gameObject.GetComponent<RoomInformation>().centrePoint.x,
                30f,
                explodeRoom.gameObject.GetComponent<RoomInformation>().centrePoint.z);
            roomCamera.transform.LookAt(explodeRoom.gameObject.GetComponent<RoomInformation>().centrePoint);
            roomCamera.enabled = true;
            roomCamera.GetComponent<AudioListener>().enabled = true;
            explodeRoom.ReassembleRoom(explodeRoom.gameObject.GetComponent<RoomInformation>().Type == global::Room.RoomType.Boss);
            yield return null;
            yield return new WaitUntil(() => explodeRoom.IsReassembled);
            roomCamera.enabled = false;
            roomCamera.GetComponent<AudioListener>().enabled = false;
        }

        explodeRoom.GetComponent<RoomNavmesh>().BuildNavMesh();
    }
}

