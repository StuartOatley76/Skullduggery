using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls the audio for the door portal
/// </summary>

public class PortalAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        // initialises audio source
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.enabled = false;

        // subscribe listeners to door events
        Teleporter.DoorsOpened += DoorsOpenedListener;
        Teleporter.DoorsClosed += DoorsClosedListener;
    }

    // enables audio when door is opened
    private void DoorsOpenedListener(object sender, EventArgs e)
    {
        audioSource.enabled = true;
    }

    // disables audio when door is closed
    private void DoorsClosedListener(object sender, EventArgs e)
    {
        audioSource.enabled = false;
    }

    private void OnDisable()
    {
        // unsubscribe listeners
        Teleporter.DoorsOpened -= DoorsOpenedListener;
        Teleporter.DoorsClosed -= DoorsClosedListener;
    }
}
