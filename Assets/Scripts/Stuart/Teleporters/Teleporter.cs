using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle teleporting player from one room to another
/// </summary>
public class Teleporter : MonoBehaviour
{
    /// <summary>
    /// List of all teleporters in thhe level
    /// </summary>
    private static List<Teleporter> allTeleporters = new List<Teleporter>();

    /// <summary>
    /// record of whether the reset delegate is connected
    /// </summary>
    private static bool hasAddedReset = false;

    /// <summary>
    /// Event triggered when the doors open
    /// </summary>
    public static EventHandler DoorsOpened;

    /// <summary>
    /// Event triggered when thhe doors close
    /// </summary>
    public static EventHandler DoorsClosed;

    /// <summary>
    /// Event for when the portal is entered
    /// </summary>
    private static EventHandler EnterPortal;

    /// <summary>
    /// Time before doors close after entering a room
    /// </summary>
    [SerializeField] private float secondsUntilDoorClosesBehind = 0.5f;

    /// <summary>
    /// Whether the door is usable
    /// </summary>
    [SerializeField] private bool usable = false;

    /// <summary>
    /// Whether the door has been opened before
    /// </summary>
    private bool hasPreviouslyOpened = false;

    /// <summary>
    /// Public accessor to check for whether the door can be used
    /// </summary>
    public bool Usable { get { return usable; }
        set { usable = value;
                foreach (Animator animator in animators) {
                    animator.SetBool("Open", value);
            }
            if(value == true && connectedTo != null && connectedTo.Usable == false) {
                connectedTo.isClosing = true;
                connectedTo.Usable = true;
            }
            if(value == true && isClosing == false) {
                hasPreviouslyOpened = true;
                DoorsOpened?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// The navmesh in the room
    /// </summary>
    public RoomNavmesh roomNavmesh { private get; set; }

    /// <summary>
    /// Whether the door is closing
    /// </summary>
    private bool isClosing = false;

    /// <summary>
    /// Room.Door that this is the physical version of
    /// </summary>
    public Room.Door ThisDoor {get; set; }

    /// <summary>
    /// Array of animators for opening and closing the door
    /// </summary>
    private Animator[] animators;

    /// <summary>
    /// Scale the player starts at
    /// </summary>
    private Vector3 playerDefaultScale;

    /// <summary>
    /// Rotation the player starts at
    /// </summary>
    private Quaternion playerDefaultRotation;

    /// <summary>
    /// Time the player takes to shrink into the portal
    /// </summary>
    private float scaletime = 3f;

    /// <summary>
    /// how far through the shrinking the player currently is
    /// </summary>
    private float currentScaleTime;

    /// <summary>
    /// Whether the player is leaving through the door
    /// </summary>
    private bool exiting = false;

    /// <summary>
    /// The level's camera fade
    /// </summary>
    private CameraFade cameraFade;

    /// <summary>
    /// The camera to view room creation with
    /// </summary>
    private static Camera roomCamera;

    /// <summary>
    /// The main camera
    /// </summary>
    private static Camera mainCamera;

    /// <summary>
    /// Whether the next room is being assembled
    /// </summary>
    private bool isAssemblingRoom;

    /// <summary>
    /// Teleporter this connects to
    /// </summary>
    [SerializeField] private Teleporter connectedTo = null;

    /// <summary>
    /// Accessor for which teleporter this is connected to 
    /// </summary>
    private Teleporter ConnectedTo {
        get { return connectedTo; }
        set {
            connectedTo = value;
        }
    }

    /// <summary>
    /// The point the player is moved to coming in through this door
    /// </summary>
    [SerializeField] private Transform arrivalPoint;

    /// <summary>
    /// The room the door is in
    /// </summary>
    [SerializeField] private GameObject room;
    public GameObject Room { get { return room; } set { room = value; } }

    /// <summary>
    /// Static event to connect all the doors up once the level creation is complete
    /// </summary>
    public static void ConnectDoors() {
        for(int i = 0; i < allTeleporters.Count; i++) {
            for(int j = 0; j < allTeleporters.Count; j++) {
                if (allTeleporters[i].ThisDoor.GetConnectedDoor().Equals(allTeleporters[j].ThisDoor)) {
                    allTeleporters[i].SetConnection(allTeleporters[j]);
                }
            }
        }
    }

    /// <summary>
    /// Initialises
    /// </summary>
    private void Awake() {
        if (!hasAddedReset) {
            SceneManager.sceneUnloaded += Reset;
            hasAddedReset = true;
        }
        allTeleporters.Add(this);
        animators = GetComponentsInChildren<Animator>();
        cameraFade = FindObjectOfType<CameraFade>();
    }
    private void SetConnection(Teleporter connection) {
        connectedTo = connection;
    }

    /// <summary>
    /// Lets the character controller know to show the button prompt
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && usable) {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController) {
                characterController.ToggleButtonPrompt(true);
            }
        }
    }

    /// <summary>
    /// Starts the player going through the door when the button is pressed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Enemy")) {
            return;
        }
        if (!Usable || isClosing ||  ConnectedTo == null) {
            return;
        }
        if(other.CompareTag("Player")) {
            if (Input.GetButton("Gamepad_A") && !exiting) {
                exiting = true;
                CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
                if (characterController) {
                    characterController.ToggleButtonPrompt(false);
                }
                StartCoroutine(LeaveRoom(other.gameObject));
            }
        }
    }

    /// <summary>
    /// Lets the character controller know to hide the button prompt
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController) {
                characterController.ToggleButtonPrompt(false);
            }
        }
    }

    /// <summary>
    /// Closes the door
    /// </summary>
    /// <returns></returns>
    private IEnumerator CloseDoor() {
        yield return new WaitForSeconds(secondsUntilDoorClosesBehind);
        DoorsClosed?.Invoke(this, EventArgs.Empty);
        connectedTo.Usable = false;
        connectedTo.isClosing = false;
    }

    /// <summary>
    /// Clean up
    /// </summary>
    public void OnDisable() {
        allTeleporters.Remove(this);
    }

    /// <summary>
    /// Animates the player moving into the portal, waits for room assembly if necessary, then teleports the player
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator LeaveRoom(GameObject player) {

        playerDefaultScale = player.transform.localScale;
        Vector3 position = player.transform.position;
        currentScaleTime = 0;
        Vector3 destination = GetDoorCentre();
        EnterPortal?.Invoke(this, EventArgs.Empty);
        do {
            player.transform.localScale = Vector3.Lerp(playerDefaultScale, Vector3.zero, currentScaleTime / scaletime);
            player.transform.position = Vector3.Lerp(position, destination, currentScaleTime / scaletime);
            currentScaleTime += Time.deltaTime;
            yield return null;
        } while (currentScaleTime < scaletime);

        if(cameraFade == null) {
            cameraFade = FindObjectOfType<CameraFade>();
        }
        cameraFade.ToggleFade();

        do {
            yield return null;
        } while (cameraFade.Fading);


        yield return WaitForRoomAssembly();

        MapCreator.ExitRoom();
        player.transform.localScale = playerDefaultScale;
        player.transform.rotation = playerDefaultRotation;
        player.transform.position = ConnectedTo.arrivalPoint.position;
        cameraFade.ToggleFade();

        do {
            yield return null;
        } while (cameraFade.Fading);

        if (connectedTo.hasPreviouslyOpened == false) {
            StartCoroutine(CloseDoor());
        }
        if (connectedTo && connectedTo.Room && connectedTo.Room.GetComponent<RoomInformation>()) {
            MapCreator.EnterRoom(connectedTo.Room.GetComponent<RoomInformation>().MapObject);
        }
        EnemySpawner spawner = ConnectedTo.Room.GetComponent<EnemySpawner>();
        if (spawner != null) {
            spawner.SpawnEnemies();
        }
        exiting = false;
    }

    /// <summary>
    /// Handles triggering room assembly cutscene if needed
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRoomAssembly() {

        ExplodeRoom explodeRoom = connectedTo.transform.root.gameObject.GetComponent<ExplodeRoom>();
        if (roomCamera == null) {
            if(GameObject.FindGameObjectWithTag("RoomCreationCamera") == null) {
                yield break;
            }
            roomCamera = GameObject.FindGameObjectWithTag("RoomCreationCamera").GetComponent<Camera>();
        }
        if(mainCamera == null) {
            mainCamera = Camera.main;
        }
        if (explodeRoom != null && connectedTo != null && roomCamera != null && !explodeRoom.HasBeenAssembled) {
            isAssemblingRoom = true;
            roomCamera.transform.position = new Vector3(
                connectedTo.transform.root.gameObject.GetComponent<RoomInformation>().centrePoint.x,
                30f,
                connectedTo.transform.root.gameObject.GetComponent<RoomInformation>().centrePoint.z);
            roomCamera.transform.LookAt(connectedTo.transform.root.gameObject.GetComponent<RoomInformation>().centrePoint);
            SwitchToCamera(false);
            explodeRoom.ReassembleRoom(connectedTo.transform.root.gameObject.GetComponent<RoomInformation>().Type == global::Room.RoomType.Boss);
            yield return null;
            yield return new WaitUntil(() => explodeRoom.IsReassembled || !isAssemblingRoom);
            if (isAssemblingRoom) {
                cameraFade.ToggleFade();
                yield return new WaitUntil(() => !cameraFade.Fading);
                roomCamera.enabled = false;
                mainCamera.enabled = true;
                isAssemblingRoom = false;
            }
        }
    }

    /// <summary>
    /// Switches between cameras
    /// </summary>
    /// <param name="switchToMainCamera"></param>
    private void SwitchToCamera(bool switchToMainCamera = true) {
        if(mainCamera == null || roomCamera == null) {
            return;
        }
        cameraFade.ToggleFade();
        if(switchToMainCamera == false) {
            mainCamera.enabled = false;
            roomCamera.enabled = true;
            return;
        }
        mainCamera.enabled = true;
        roomCamera.enabled = false;
    }

    /// <summary>
    /// Finds the centre of the door for portal animation
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDoorCentre() {
        GameObject centrepiece = FindChildWithTag("DoorCentre");
        if(centrepiece == null) {
            return Vector3.zero;
        }
        return centrepiece.transform.position;
    }

    /// <summary>
    /// starts tree search for child object with specified tag
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private GameObject FindChildWithTag(string tag) {
        return GetChild(transform, tag);
    }

    /// <summary>
    /// performs recursive tree search for child object with specified tag
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="tag"></param>
    /// <param name="foundGameObject"></param>
    /// <returns></returns>
    private GameObject GetChild(Transform parent, string tag, GameObject foundGameObject = null) {
        for(int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child.tag == tag) {
                foundGameObject = child.gameObject;
                return child.gameObject;
            }
            if(child.childCount > 0) {
                foundGameObject = GetChild(child, tag, null);
            }
            if(foundGameObject != null) {
                return foundGameObject;
            }
        }
        return null;
    }


    /// <summary>
    /// resets static variables
    /// </summary>
    /// <param name="scene"></param>
    private static void Reset(Scene scene) {
        allTeleporters.Clear();
        hasAddedReset = false;
        SceneManager.sceneUnloaded -= Reset;
    }
}
