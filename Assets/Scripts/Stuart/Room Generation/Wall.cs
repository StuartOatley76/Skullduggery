using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle wall occlusion
/// </summary>
[RequireComponent(typeof(Renderer))]
public class Wall : MonoBehaviour
{

    /// <summary>
    /// Layer for walls
    /// </summary>
    public static int WallLayer { get; private set; }

    /// <summary>
    /// Whether the wall should be occluded. Set by shader
    /// </summary>
    public bool Occluded { private get; set; }

    /// <summary>
    /// Parent gameobject
    /// </summary>
    private GameObject parent;

    /// <summary>
    /// Initialises
    /// </summary>
    private void Awake() {
        WallLayer = LayerMask.NameToLayer("Wall");
        parent = transform.parent.gameObject;
    }

    /// <summary>
    /// Sets visibility based on occlusion
    /// </summary>
    private void LateUpdate() {
        foreach(Transform child in parent.transform) {
            if(child.gameObject.layer == WallLayer) {
                if (child.gameObject.GetComponent<Renderer>()) {
                    child.gameObject.GetComponent<Renderer>().enabled = !Occluded;
                }
            }
        }
        Occluded = false;
    }
}
