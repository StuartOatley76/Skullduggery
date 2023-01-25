using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to hold references to the renderers that need to have materials changed based on the type of room
/// </summary>
public class RendererReferences : MonoBehaviour
{
    [SerializeField] private List<Renderer> wallRenderers;
    public List<Renderer> WallRenderer { get { return wallRenderers; } }

    [SerializeField] private List<Renderer> floorRenderers;
    public List<Renderer> FloorRenderer { get { return floorRenderers; } }

    [SerializeField] private List<Renderer> pillarRenderers;
    public List<Renderer> PillarRenderers { get { return pillarRenderers; } }
}
