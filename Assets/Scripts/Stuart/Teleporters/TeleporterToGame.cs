using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Teleporter to handle teleporting into new level
/// </summary>
public class TeleporterToGame : MonoBehaviour
{

    /// <summary>
    /// Scale values for portal animation
    /// </summary>
    private Vector3 playerDefaultScale;
    private float scaletime = 3f;
    private float currentScaleTime;

    /// <summary>
    /// Is the player exiting through the portal
    /// </summary>
    private bool exiting = false;

    /// <summary>
    /// The level's camera fade
    /// </summary>
    private CameraFade cameraFade;

    /// <summary>
    /// Animators to open and close the door
    /// </summary>
    protected Animator[] animators;

    /// <summary>
    /// Whether the door can be used
    /// </summary>
    [SerializeField] private bool usable = false;
    public bool Usable {
        get { return usable; }
        set {
            usable = value;
            foreach (Animator animator in animators) {
                animator.SetBool("Open", value);
            }
        }
    }
    /// <summary>
    /// The room's gameobject
    /// </summary>
    public GameObject Room { get; set; }

    /// <summary>
    /// finds the animators
    /// </summary>
    protected void Awake() {
        animators = GetComponentsInChildren<Animator>();
    }

    /// <summary>
    /// Tells the character controller to display the button prompt
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
    /// Triggers the player going to the next level when the button is pressed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Enemy")) {
            return;
        }

        if (other.CompareTag("Player")) {
            if (Input.GetButton("Gamepad_A") && !exiting && usable) {
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
    /// Tells the character controller to hide the button prompt
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
    /// performs portal animation then loads the next scene
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    protected virtual IEnumerator LeaveRoom(GameObject player) {

        RoomLayout.NewLevel();
        playerDefaultScale = player.transform.localScale;
        Vector3 position = player.transform.position;
        currentScaleTime = 0;
        Vector3 destination = GetDoorCentre();
        do {
            player.transform.localScale = Vector3.Lerp(playerDefaultScale, Vector3.zero, currentScaleTime / scaletime);
            player.transform.position = Vector3.Lerp(position, destination, currentScaleTime / scaletime);
            currentScaleTime += Time.deltaTime;
            yield return null;
        } while (currentScaleTime < scaletime);

        if (cameraFade == null) {
            cameraFade = FindObjectOfType<CameraFade>();
        }
        cameraFade.ToggleFade();

        do {
            yield return null;
        } while (cameraFade.Fading);

        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Finds the centre of the door for the portal animation
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDoorCentre() {
        GameObject centrepiece = FindChildWithTag("DoorCentre");
        if (centrepiece == null) {
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
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child.tag == tag) {
                foundGameObject = child.gameObject;
                return child.gameObject;
            }
            if (child.childCount > 0) {
                foundGameObject = GetChild(child, tag, null);
            }
            if (foundGameObject != null) {
                return foundGameObject;
            }
        }
        return null;
    }
}
