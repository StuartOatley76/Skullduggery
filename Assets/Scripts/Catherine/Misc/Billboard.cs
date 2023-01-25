using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the billboarding effect for 2D objects in the 3D scene
/// </summary>

public class Billboard : MonoBehaviour
{
    private Camera billboardCamera;

    void Start()
    {
        // initialise camera
        billboardCamera = Camera.main;

        // ensures ingame sprites render first
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null && renderer.material != null) {
            renderer.material.renderQueue = 4500;
        }
    }

    void LateUpdate()
    {
        // set camera on update, as camera changes between rooms
        billboardCamera = Camera.main;

        if (billboardCamera)
        {
            // sets the object's rotation as the camera rotation
            transform.rotation = billboardCamera.transform.rotation;
            // rotate the object towards the camera on the y axis
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
