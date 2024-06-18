using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KimetsuSoundController : MonoBehaviour
{

    // Import AudioSource Component
    private AudioSource audioSource;
    
    void Awake()
    {
        // Assign audioSource to the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }
    
    // Play Footstep sound when FootstepSound is called
    void FootstepSound()
    {
        audioSource.Play();
    }
}
