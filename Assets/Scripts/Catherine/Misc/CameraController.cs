using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the ingame camera
/// </summary>

public class CameraController : MonoBehaviour
{
    private Quaternion cameraRotation;

    private void Awake()
    {
        // store initial rotation
        cameraRotation = gameObject.transform.rotation;
    }

    void LateUpdate()
    {
        // maintain original rotation, otherwise will rotate with player rotation
        gameObject.transform.rotation = cameraRotation.normalized;
    }
}