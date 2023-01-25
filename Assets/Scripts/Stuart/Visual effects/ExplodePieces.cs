using System.Collections;
using UnityEngine;

/// <summary>
/// Class to handle moving room piece and then animating back to it's position
/// </summary>
public class ExplodePieces : MonoBehaviour {

    /// <summary>
    /// Minimum distance to move the piece
    /// </summary>
    const float minExplodeDistance = 30f;

    /// <summary>
    /// Maximum distance to move the piece
    /// </summary>
    const float maxExplodeDistance = 50f;

    /// <summary>
    /// Time to take to move back
    /// </summary>
    const float secondsToReassemble = 4f;

    /// <summary>
    /// The piece's starting position
    /// </summary>
    private Vector3 startingPosition;

    /// <summary>
    /// The pieces starting rotation
    /// </summary>
    private Quaternion startingRotation;

    /// <summary>
    /// The position the piece moved to
    /// </summary>
    private Vector3 explodedPosition;

    /// <summary>
    /// The rotation the piece moved to
    /// </summary>
    private Quaternion explodedRotation;

    /// <summary>
    /// The time the reassembly started
    /// </summary>
    private float startingTime;

    /// <summary>
    /// Whether the animation has already run
    /// </summary>
    private bool hasExploded = false;
    public bool IsAssembled { get; private set; } = true;

    /// <summary>
    /// Moves the piece to a random position and rotation then starts the coroutine to move it back
    /// </summary>
    public void Explode() {
        if (hasExploded) {
            return;
        }
        RoomInformation information = transform.root.gameObject.GetComponent<RoomInformation>();
        Vector3 centrePoint = information.centrePoint;
        int xSign = Random.Range(-1, 2);
        xSign = (xSign == 0) ? 1 : xSign;
        int ySign = Random.Range(-1, 2);
        ySign = (ySign == 0) ? 1 : ySign;
        int zSign = Random.Range(-1, 2);
        zSign = (xSign == 0) ? 1 : zSign;
        float xExplodeDistance = Random.Range(minExplodeDistance, maxExplodeDistance) * xSign;
        float yExplodeDistance = Random.Range(minExplodeDistance, maxExplodeDistance) * ySign;
        float zExplodeDistance = Random.Range(minExplodeDistance, maxExplodeDistance) * zSign;
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        hasExploded = true;
        IsAssembled = false;
        explodedPosition = transform.position = new Vector3(transform.position.x + xExplodeDistance, 0, transform.position.z + zExplodeDistance);
        explodedRotation = transform.rotation = Random.rotation;
        StartCoroutine(Reassemble());
    }

    /// <summary>
    /// Coroutine to Slerp the piece back to it's original position and rotation over the set amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reassemble() {
        startingTime = Time.time;
        while (startingTime + secondsToReassemble > Time.time) {
            if (IsAssembled) {
                yield break;
            }
            transform.position = Vector3.Slerp(explodedPosition, startingPosition, (Time.time - startingTime) / secondsToReassemble);
            transform.rotation = Quaternion.Slerp(explodedRotation, startingRotation, (Time.time - startingTime) / secondsToReassemble);
            yield return null;
        }
        FinishAssembly();
    }

    /// <summary>
    /// Slerp never gets fully to it's destination so we finish by putting it directly to it's original state
    /// </summary>
    public void FinishAssembly() {
        IsAssembled = true;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
    }
}