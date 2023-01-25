using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys the Score Feedback after a set time
/// </summary>

public class ScoreFeedback : MonoBehaviour
{
    [SerializeField] private float lifetime;

    void Start()
    {
        // destroy object after lifetime
        Destroy(gameObject, lifetime);   
    }
}
