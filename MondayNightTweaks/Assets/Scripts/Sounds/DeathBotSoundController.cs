using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBotSoundController : MonoBehaviour
{
    // Import audio source components
    public AudioSource audioSourceWalk;
    public AudioSource audioSourceRun;
    public AudioSource audioSourceAU;
    public AudioSource audioSourceAD;

    
    // Play walk sound when Footstep flag is called
    void Footstep()
    {
        audioSourceWalk.Play();
    }
    
    // Play run sound when Runstep flag is called
    void Runstep()
    {
        audioSourceRun.Play();
    }

    // Play ArmsUp sound when ArmsUp flag is called
    void ArmsUp()
    {
        audioSourceAU.Play();
    }

    // Play ArmsDown (Game Over) sound when ArmsDown flag is called
    void ArmsDown()
    {
        audioSourceAD.Play();
    }
}
